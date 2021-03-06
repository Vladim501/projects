
--Определить у каких продавцов (EmployeeID) делали заказы в марте и апреле 1997 года покупатели (CustomerID) из запроса номер 1.
--Очень прошу вашей помощи.


SELECT DISTINCT CUSTOMERID 
FROM ORDERS WHERE EMPLOYEEID IN 
(
    SELECT EmployeeID 
    FROM orders
    WHERE YEAR (OrderDate) = 1997 AND MONTH (OrderDate) = 3 
    EXCEPT
    SELECT EmployeeID 
    FROM orders
    WHERE YEAR (OrderDate) = 1997 AND MONTH (OrderDate) = 4
) 
AND MONTH(OrderDate) = 3