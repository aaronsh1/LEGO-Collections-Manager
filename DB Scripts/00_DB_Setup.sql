USE master;
GO
IF DB_ID (N'LegoDB') IS NOT NULL
DROP DATABASE LegoDB;
GO

CREATE DATABASE LegoDB;
GO

USE LegoDB;
GO

CREATE TABLE [PieceCategory] (
  [PieceCategoryId] int PRIMARY KEY,
  [PieceCategoryName] varchar(255)
)
GO

CREATE TABLE [User] (
  [UserId] int PRIMARY KEY IDENTITY(1, 1),
  [Username] varchar(255)
)
GO

CREATE TABLE [Colour] (
  [ColourId] int PRIMARY KEY,
  [ColourName] varchar(255)
)
GO

CREATE TABLE [Piece] (
  [PieceId] varchar(50) PRIMARY KEY,
  [PieceName] varchar(255),
  [PieceCategory] int,

   FOREIGN KEY ([PieceCategory]) REFERENCES [PieceCategory] ([PieceCategoryId])
  ON DELETE CASCADE
)
GO

CREATE TABLE [SetCategory] (
  [SetCategoryId] int PRIMARY KEY,
  [SetCategoryName] varchar(255)
)
GO

CREATE TABLE [Set] (
  [SetId] int PRIMARY KEY,
  [SetName] varchar(255),
  [PiecesAmount] int,
  [SetCategory] int,

   FOREIGN KEY ([SetCategory]) REFERENCES [SetCategory] ([SetCategoryId])
  ON DELETE CASCADE
)
GO

CREATE TABLE [SetPiece] (
  [SetPieceId] int PRIMARY KEY IDENTITY(1, 1),
  [Piece] varchar(50),
  [SetId] int,
  [Amount] int,
  [Colour] int,

   FOREIGN KEY ([Piece]) REFERENCES [Piece] ([PieceId])
  ON DELETE CASCADE,
   FOREIGN KEY ([SetId]) REFERENCES [Set] ([SetId])
  ON DELETE CASCADE,
   FOREIGN KEY ([Colour]) REFERENCES [Colour] ([ColourId])
)
GO



CREATE TABLE [UserSparePieces] (
  [User] int,
  [Piece] varchar(50),
  [Amount] int,
  [Colour] int,

   FOREIGN KEY ([User]) REFERENCES [User] ([UserId])
  ON DELETE CASCADE,
   FOREIGN KEY ([Piece]) REFERENCES [Piece] ([PieceId])
  ON DELETE CASCADE,
   FOREIGN KEY ([Colour]) REFERENCES [Colour] ([ColourId])
)
GO

CREATE TABLE [UserSet] (
  [UseSetId] int PRIMARY KEY IDENTITY(1, 1),
  [User] int,
  [Set] int,

   FOREIGN KEY ([User]) REFERENCES [User] ([UserId])
  ON DELETE CASCADE,
   FOREIGN KEY ([Set]) REFERENCES [Set] ([SetId])
  ON DELETE CASCADE
)
GO


CREATE TABLE [MissingPiece] (
  [MissingPieceId] int PRIMARY KEY IDENTITY(1, 1),
  [Piece] varchar(50),
  [UserSet] int,
  [Amount] int,
  [Colour] int,

   FOREIGN KEY ([UserSet]) REFERENCES [UserSet] ([UseSetId])
  ON DELETE CASCADE,
   FOREIGN KEY ([Piece]) REFERENCES [Piece] ([PieceId])
  ON DELETE CASCADE,
   FOREIGN KEY ([Colour]) REFERENCES [Colour] ([ColourId])
)
GO

CREATE TABLE [SetPieceCategory] (
  [SetPieceCategoryId] int PRIMARY KEY IDENTITY(1, 1),
  [PieceCategory] int,
  [Set] int,

   FOREIGN KEY ([PieceCategory]) REFERENCES [PieceCategory] ([PieceCategoryId])
  ON DELETE CASCADE,
   FOREIGN KEY ([Set]) REFERENCES [Set] ([SetId])
  ON DELETE CASCADE
)
GO