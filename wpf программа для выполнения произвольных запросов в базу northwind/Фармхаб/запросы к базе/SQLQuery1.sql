
--Для каждого товара (вывести имя товара) показать два города, в которые его больше всего заказали.

SELECT P.ProductName, ShipCity, cnt FROM

(
	SELECT  OD.ProductID, O.ShipCity, SUM(Quantity) cnt,
	DENSE_RANK() OVER( PARTITION BY OD.ProductID ORDER BY SUM(Quantity) DESC) ord
	FROM [Order Details] OD
	INNER JOIN [Orders] O
	ON OD.OrderID=O.OrderID
	GROUP BY OD.ProductID,O.ShipCity
) Z
JOIN dbo.Products P 
	ON z.ProductID=P.ProductID
WHERE Z.ord IN (1,2)
