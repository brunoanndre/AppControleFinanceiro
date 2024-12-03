--Get Incomes and Expenses
CREATE OR ALTER VIEW vwGetIncomesAndExpenses AS
    SELECT 
        t.UserId,
        MONTH(t.PaidOrReceivedAt) as Month,
        YEAR(t.PaidOrReceivedAt) as Year,
        SUM(CASE WHEN t.Type = 1 THEN t.Amount ELSE 0 END) as Incomes,
        SUM(CASE WHEN t.[Type] = 2 THEN t.Amount ELSE 0 END) as Expenses
    FROM    
        [Transaction] t
    WHERE
        t.PaidOrReceivedAt >= DATEADD(MONTH, -11,CAST(GETDATE() AS DATE))
        AND t.PaidOrReceivedAt < DATEADD(MONTH, 1,CAST(GETDATE() AS DATE))
    GROUP BY
        t.UserId,MONTH(t.PaidOrReceivedAt),YEAR(t.PaidOrReceivedAt) 

