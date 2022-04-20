Use LegoDB
GO

BULK INSERT SetPiece
FROM 'C:\Users\schej002\Desktop\02_lego_db_import.csv'
WITH (
  FIELDTERMINATOR = ',',
  ROWTERMINATOR = '\n',
  FIRSTROW = 2
);