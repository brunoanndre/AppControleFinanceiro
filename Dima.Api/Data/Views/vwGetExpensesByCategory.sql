--Get expenses by Category
CREATE OR ALTER VIEW vwGetExpensesByCategory AS
    SELECT 
        t.UserId,
        c.Title AS Category,
        YEAR(t.PaidOrReceivedAt) AS [Year],
        SUM(t.Amount) AS [Expenses]
    FROM    
        [Transaction] t
        INNER JOIN Category c ON t.CategoryId = c.Id
    WHERE
        t.PaidOrReceivedAt >= DATEADD(MONTH, -11,CAST(GETDATE() AS DATE))
        AND t.PaidOrReceivedAt < DATEADD(MONTH, 1,CAST(GETDATE() AS DATE))
        AND t.[Type] = 2
    GROUP BY
        t.UserId,c.Title,YEAR(t.PaidOrReceivedAt)


