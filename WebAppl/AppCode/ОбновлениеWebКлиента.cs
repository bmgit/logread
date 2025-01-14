﻿using System;
using System.Data;
using System.IO;
using System.Configuration;
using Барс.Клиент;

public static class ОбновлениеWebКлиента
{
    public static void ОбновитьКлиент()
    {
        ОбновлениеИсходныхКодов обновлениеИсходныхТекстов = new ОбновлениеИсходныхКодов();

        bool требуетсяОбновление = false;

        try
        {
            требуетсяОбновление = обновлениеИсходныхТекстов.ПроверитьНаОбновление(false);
        }
        catch
        {
            return;
        }

        bool необходимаКомпиляция = false;

        if (требуетсяОбновление)
        {
            необходимаКомпиляция = true;

			#region Выполнение обновления исходных текстов
			try
			{
				if( обновлениеИсходныхТекстов == null )
				{
					return;
				}

				обновлениеИсходныхТекстов.ЗагрузитьОбновление();
			}
			catch
			{
			}
			#endregion
		}
		else
		{
			необходимаКомпиляция = false;
        }

        if (!необходимаКомпиляция)
        {
            // проверяем, а существуют ли необходимые библиотеки
            foreach (КомпонентСистемы компонент in Приложение.ПолучитьВсеКомпоненты())
            {
                if (!File.Exists( Path.Combine( Приложение.РабочаяПапка, компонент.Имя + ".dll" )))
                {
                    необходимаКомпиляция = true;
                    break;
                }
            }
        }

        // компиляция исходных текстов
        if (необходимаКомпиляция)
        {
            КомпиляторПроекта компилятор = new КомпиляторПроекта();

            if (!компилятор.ВыполнитьКомпиляцию(null))
            {
                return;
            }
        }
    }
}
