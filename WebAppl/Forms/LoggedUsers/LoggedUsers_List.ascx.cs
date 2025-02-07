﻿using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Барс.ВебЯдро;
using Барс.ВебЯдро.Интерфейс;
using Барс.ВебЯдро.РаботающиеПользователи;

public partial class Forms_Admin_LoggedUsers_List : Барс.ВебЯдро.Интерфейс.ВебФорма
{
	public Forms_Admin_LoggedUsers_List()
		: base()
	{
		this.ЗаголовокСтраницы = "Список работающих пользователей";
		this.ШапкаСтраницы = "Список пользователей, работающих в данный момент в системе";
		this.ПриИнициализацииСтраницы += new Барс.Интерфейс.ОбработчикСобытия( Forms_Admin_LoggedUsers_List_ПриИнициализацииСтраницы );
		this.PreRender += new EventHandler( Forms_Admin_LoggedUsers_List_PreRender );
	}

	void Forms_Admin_LoggedUsers_List_PreRender( object sender, EventArgs e )
	{
		if( СообщениеАдминистратора.ЕстьСообщение )
		{
			Надпись_ТекущееСообщение.Text = СообщениеАдминистратора.ТекстСообщения;
			Panel_Message_Minus.Visible = true;
			Panel_Message_Plus.Visible = false;
		}
		else
		{
			Panel_Message_Minus.Visible = false;
			Panel_Message_Plus.Visible = true;
		}
	}

	void Forms_Admin_LoggedUsers_List_ПриИнициализацииСтраницы( object Отправитель, Барс.Интерфейс.АргументыСобытия Аргументы )
	{

		List<ОписаниеРаботающегоПользователя> списокПользователей;

		Application.Lock();
		списокПользователей = ( Application [ "РаботающиеПользователи" ] as СписокРаботающихПользователей ).ПолучитьСписокПользователей();
		Application.UnLock();

		Таблица_РаботающиеПользователи.ИсточникЗаписей = списокПользователей;

		if( !IsPostBack )
		{
			// выставляем столбцы таблицы
			СтолбецТаблицы столбец;

			столбец = new СтолбецТаблицы();
			Таблица_РаботающиеПользователи.ДобавитьСтолбец( столбец );

			столбец.Заголовок = "Пользователь";
			столбец.ИмяПоляИсточникаДанных = "ИмяПользователя";

			столбец = new СтолбецТаблицы();
			Таблица_РаботающиеПользователи.ДобавитьСтолбец( столбец );

			столбец.Заголовок = "Роль в системе";
			столбец.ИмяПоляИсточникаДанных = "РолиПользователя";

			столбец = new СтолбецТаблицы();
			Таблица_РаботающиеПользователи.ДобавитьСтолбец( столбец );

			столбец.Заголовок = "Начало работы";
			столбец.ИмяПоляИсточникаДанных = "ВремяНачалаРаботы";
			

			столбец = new СтолбецТаблицы();
			Таблица_РаботающиеПользователи.ДобавитьСтолбец( столбец );

			столбец.Заголовок = "Хост";
			столбец.ИмяПоляИсточникаДанных = "Хост";
			

			столбец = new СтолбецТаблицы();
			Таблица_РаботающиеПользователи.ДобавитьСтолбец( столбец );

			столбец.Заголовок = "IP адрес";
			столбец.ИмяПоляИсточникаДанных = "IP";
			

			столбец = new СтолбецТаблицы();
			Таблица_РаботающиеПользователи.ДобавитьСтолбец( столбец );

			столбец.Заголовок = "Браузер";
			столбец.ИмяПоляИсточникаДанных = "UserAgent";
		}
	}

	protected void Кнопка_УстановитьСообщение_Click( object sender, EventArgs e )
	{
		СообщениеАдминистратора.УстановитьСообщение( ПолеВводаТекста_ТекстСообщения.Текст, (int) ПолеВводаЧисла_СрокДействия.Значение );
	}

	protected void Кнопка_УдалитьТекущееСообщение_Click( object sender, EventArgs e )
	{
		СообщениеАдминистратора.УбратьСообщение();
	}

    protected void Кнопка_Обновить_Click(object sender, EventArgs e)
    {
        Таблица_РаботающиеПользователи.Rebind();
    }
}
