
--Все Клиенты, которые потратили более 25 000 долларов в заказах с Northwind

select a.customerid, sum (b.unitprice*b.quantity) as Total_Amount 
from orders a join [order details] b on a.orderid=b.orderid 
group by a.customerid 
having sum (b.unitprice*b.quantity) > 25000 
order by sum(b.unitprice*b.quantity) desc 
