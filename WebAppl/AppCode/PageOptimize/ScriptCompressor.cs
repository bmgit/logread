using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Web.Caching;
using System.Net;
using System.Collections.Generic;

public class ScriptCompressorHandler : IHttpHandler
{
    private const int DAYS_IN_CACHE = 30;

	/// <summary>
	/// Enables processing of HTTP Web requests by a custom 
	/// HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
	/// </summary>
	/// <param name="context">An <see cref="T:System.Web.HttpContext"></see> object that provides 
	/// references to the intrinsic server objects 
	/// (for example, Request, Response, Session, and Server) used to service HTTP requests.
	/// </param>
	public void ProcessRequest(HttpContext context)
	{
		string root = context.Request.Url.GetLeftPart(UriPartial.Authority);
		string path = context.Request.QueryString["path"];
		string content = string.Empty;
		List<string> localFiles = new List<string>();

		if (!string.IsNullOrEmpty(path))
		{
			string[] scripts = path.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string script in scripts)
			{
                content += "/*---" + script + "---*/";
				// We only want to serve resource files for security reasons.
				if (script.Contains("Resource.axd") || script.Contains("asmx/js") || script.StartsWith("http", StringComparison.OrdinalIgnoreCase))
					content += RetrieveRemoteScript(root + script) + Environment.NewLine;
				else
					content += RetrieveLocalScript(script, localFiles) + Environment.NewLine;
			}

            
			//content = StripWhitespace(content);
		}

		if (!string.IsNullOrEmpty(content))
		{
			context.Response.Write(content);
			SetHeaders(context, localFiles.ToArray());

            PageOptimizer.Compress(context);
		}
	}

	/// <summary>
	/// Retrieves the local script from the disk
	/// </summary>
	private static string RetrieveLocalScript(string file, List<string> localFiles)
	{
		if (!file.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
		{
			throw new System.Security.SecurityException("No access");
		}

		string path = HttpContext.Current.Server.MapPath(file);
		string script = null;

		if (File.Exists(path))
		{
			using (StreamReader reader = new StreamReader(path))
			{
                Miron.Web.MbCompression.JavaScriptMinifier jsMinifier = new Miron.Web.MbCompression.JavaScriptMinifier();
                script = jsMinifier.Minify(reader);
                //script = reader.ReadToEnd();
			}

			localFiles.Add(path);
		}

		return script;
	}

	/// <summary>
	/// Retrieves the specified remote script using a WebClient.
	/// </summary>
	/// <param name="file">The remote URL</param>
	private static string RetrieveRemoteScript(string file)
	{
		string script = null;

		try
		{
			Uri url = new Uri(file, UriKind.Absolute);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				script = reader.ReadToEnd();
			}
		}
		catch (System.Net.Sockets.SocketException)
		{
			// The remote site is currently down. Try again next time.
		}
		catch (UriFormatException)
		{
			// Only valid absolute URLs are accepted
		}

		return script;
	}

	/// <summary>
	/// Strips the whitespace from any .js file.
	/// </summary>
	private static string StripWhitespace(string body)
	{
		string[] lines = body.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		StringBuilder emptyLines = new StringBuilder();
		foreach (string line in lines)
		{
			string s = line.Trim();
			if (s.Length > 0 && !s.StartsWith("//"))
				emptyLines.AppendLine(s.Trim());
		}

		body = emptyLines.ToString();

		// remove C styles comments
		body = Regex.Replace(body, "/\\*.*?\\*/", String.Empty, RegexOptions.Compiled | RegexOptions.Singleline);
		//// trim left
		body = Regex.Replace(body, "^\\s*", String.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
		//// trim right
		body = Regex.Replace(body, "\\s*[\\r\\n]", "\r\n", RegexOptions.Compiled | RegexOptions.ECMAScript);
		// remove whitespace beside of left curly braced
		body = Regex.Replace(body, "\\s*{\\s*", "{", RegexOptions.Compiled | RegexOptions.ECMAScript);
		// remove whitespace beside of coma
		body = Regex.Replace(body, "\\s*,\\s*", ",", RegexOptions.Compiled | RegexOptions.ECMAScript);
		// remove whitespace beside of semicolon
		body = Regex.Replace(body, "\\s*;\\s*", ";", RegexOptions.Compiled | RegexOptions.ECMAScript);
		// remove newline after keywords
		body = Regex.Replace(body, "\\r\\n(?<=\\b(abstract|boolean|break|byte|case|catch|char|class|const|continue|default|delete|do|double|else|extends|false|final|finally|float|for|function|goto|if|implements|import|in|instanceof|int|interface|long|native|new|null|package|private|protected|public|return|short|static|super|switch|synchronized|this|throw|throws|transient|true|try|typeof|var|void|while|with)\\r\\n)", " ", RegexOptions.Compiled | RegexOptions.ECMAScript);

		return body;
	}

	/// <summary>
	/// This will make the browser and server keep the output
	/// in its cache and thereby improve performance.
	/// </summary>
	private static void SetHeaders(HttpContext context, string[] files)
	{
		//return;
		context.Response.ContentType = "text/javascript";
		context.Response.AddFileDependencies(files);
		context.Response.Cache.VaryByParams["path"] = true;
		context.Response.Cache.SetETagFromFileDependencies();
		context.Response.Cache.SetLastModifiedFromFileDependencies();
		context.Response.Cache.SetValidUntilExpires(true);
		context.Response.Cache.SetExpires(DateTime.Now.AddDays(DAYS_IN_CACHE));
		context.Response.Cache.SetCacheability(HttpCacheability.Public);
	}

	/// <summary>
	/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"></see> instance.
	/// </summary>
	/// <value></value>
	/// <returns>true if the <see cref="T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
	public bool IsReusable
	{
		get { return false; }
	}

}

/// <summary>
/// Find scripts and change the src to the ScriptCompressorHandler.
/// </summary>
public class ScriptCompressorModule : IHttpModule
{

	#region IHttpModule Members

	void IHttpModule.Dispose()
	{
		// Nothing to dispose; 
	}

	void IHttpModule.Init(HttpApplication context)
	{
		context.PostRequestHandlerExecute += new EventHandler(context_BeginRequest);
	}

	#endregion

	void context_BeginRequest(object sender, EventArgs e)
	{
		HttpApplication app = sender as HttpApplication;
		if (app.Context.CurrentHandler is Page)
		{
			app.Response.Filter = new WebResourceFilter(app.Response.Filter);
		}
	}

	#region Stream filter

	private class WebResourceFilter : Stream
	{
        private const string VERSION = "1.1";

		public WebResourceFilter(Stream sink)
		{
			_sink = sink;
		}

		private Stream _sink;

		#region Properites

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
			_sink.Flush();
		}

		public override long Length
		{
			get { return 0; }
		}

		private long _position;
		public override long Position
		{
			get { return _position; }
			set { _position = value; }
		}

		#endregion

		#region Methods

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _sink.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return _sink.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			_sink.SetLength(value);
		}

		public override void Close()
		{
			_sink.Close();
		}

        private readonly Regex regex = new Regex(@"<script\s*type=\""text/javascript\""\s*src=\""(?<src>(?!/webresource\.axd|scriptresource\.axd|scriptservice\.asmx)[^""]*js)\""\>(?:</script>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public override void Write(byte[] buffer, int offset, int count)
		{
			byte[] data = new byte[count];
			Buffer.BlockCopy(buffer, offset, data, 0, count);
			string html = System.Text.Encoding.Default.GetString(buffer);
			int index = 0;
			List<string> list = new List<string>();

			foreach (Match match in regex.Matches(html))
			{
				if (index == 0)
					index = html.IndexOf(match.Value);

				string relative = match.Groups["src"].Value;
				list.Add(relative);
				html = html.Replace(match.Value, string.Empty);
			}

			if (index > 0)
			{
                string script = "<script type=\"text/javascript\" src=\"js.axd?ver={1}&path={0}\"></script>";
				string path = string.Empty;
				foreach (string s in list)
				{
					path += HttpUtility.UrlEncode(s) + ",";
				}

                path = path.Substring(0, path.Length - 1);

                html = html.Insert(index, string.Format(script, path, VERSION));
			}

			byte[] outdata = System.Text.Encoding.Default.GetBytes(html);
			_sink.Write(outdata, 0, outdata.GetLength(0));
		}

		#endregion

	}

	#endregion

}