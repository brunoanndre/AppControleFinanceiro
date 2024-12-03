--Get Incomes by Category
CREATE OR ALTER VIEW vwGetIncomesByCategory AS
    SELECT 
        t.UserId,
        c.Title AS Category,
        YEAR(t.PaidOrReceivedAt) AS [Year],
        SUM(t.Amount) AS [Incomes]
    FROM    
        [Transaction] t
        INNER JOIN Category c ON t.CategoryId = c.Id
    WHERE
        t.PaidOrReceivedAt >= DATEADD(MONTH, -11,CAST(GETDATE() AS DATE))
        AND t.PaidOrReceivedAt < DATEADD(MONTH, 1,CAST(GETDATE() AS DATE))
        AND t.[Type] = 1
    GROUP BY
        t.UserId,c.Title,YEAR(t.PaidOrReceivedAt)

