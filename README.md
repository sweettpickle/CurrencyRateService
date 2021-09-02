# CurrencyRateService
Сервис сбора информации о курсе валют


Мы решили добавить поддержку валют в нашу информационную систему. Нам необходимо знать курсы валют, которые мы используем, 
на определённую дату. На сайте ЦБ есть вся необходимая нам информация. С сайта ЦБ берем курс валюты на дату 
такой строчкой: http://www.cbr.ru/scripts/XML_daily.asp?date_req=21.08.2019 . 
Справочник валют - http://www.cbr.ru/scripts/XML_valFull.asp. 


Нужно: 
1.       Создать таблицы для хранения информации.
2.       Написать код, который будет получать курс валюты, которая у нас используется в системе, каждый день. 
3.      Написать функцию на SQL, которая будет возвращать курс валюты на определенную дату. Функция должна принимать два параметра – валюту и дату курса, а возвращать курс.
4.      Написать, аналогичную функцию из п.3, на выбранном языке программирования. 



CREATE FUNCTION get_rate(charCode text, datte date) RETURNS real[] AS '
    select "Value"
	from "CurrencyRate" 
	inner join "Valute" on "CurrencyRate"."ValuteId" = "Valute"."Id"
	where cast("Date" as DATE) = cast(datte as DATE)
    and lower("CharCode") = lower('AUD')
' LANGUAGE SQL;

SELECT get_rate('AUD', cast (now() as DATE));