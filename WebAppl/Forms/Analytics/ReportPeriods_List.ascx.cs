using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using ����;
using ����.����;
using ����.�������;
using ����.�������.���������;
using ����.�����;


public partial class Forms_Analytics_ReportPeriods_List : ��������
{
    public Forms_Analytics_ReportPeriods_List()
        : base()
    {
        this.����������������� = "����������� ��������� �������";

        this.������������������������ += new ����.���������.�����������������(Forms_Analytics_ReportPeriods_List_������������������������);
    }

    void Forms_Analytics_ReportPeriods_List_������������������������(object �����������, ����.���������.���������������� ���������)
    {
        if (!IsPostBack)
        {
            if (Session["AnalyticExtract"] != null && Session["AnalyticExtract"] is ���������������������������)
            {
                ��������������������������� ��������������������������� = Session["AnalyticExtract"] as ���������������������������;

                if (���������������������������.������������������������)
                {
                    ���������������������� ���������������������� = new ����������������������();
                    ����������������������.���������();

                    �������_��������.��������������� = ����������������������;
                }
                else
                {
                    �������<�������������������������> ���������������������������������� = new �������<�������������������������>();
                    ����������������������������������.���������();

                    �������_��������.��������������� = ����������������������������������;
                }

                �������_��������.��������������������� = false;
                �������_��������.����������������� = false;
                �������_��������.������������������� = false;
                �������_��������.���������������� = false;

                �������������� �������_������������ = new ��������������();
                �������_��������.���������������(�������_������������);
                �������_������������.��������� = "������������";
                �������_������������.���������������������� = "������������";

                �������������� �������_������ = new ��������������();
                �������_��������.���������������(�������_������);
                �������_������.��������� = "�������� ������";
                �������_������.���������������������� = "���������������������";
            }
        }
    }
}
