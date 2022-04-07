CREATE TABLE [Piece] (
  [PieceId] int PRIMARY KEY IDENTITY(1, 1),
  [PieceName] VARCHAR,
  [PieceCategory] int,
  [PieceImage] VARCHAR
)
GO

CREATE TABLE [SubstitutePool] (
  [SubstitutePool] int PRIMARY KEY IDENTITY(1, 1),
  [Name] VARCHAR
)
GO

CREATE TABLE [SubstitutePoolItem] (
  [SubstitutePoolItemId] int PRIMARY KEY IDENTITY(1, 1),
  [Piece] int,
  [SubstitutePool] int
)
GO

CREATE TABLE [UserSubsititutePool] (
  [User] int,
  [SubstitutePool] int
)
GO

CREATE TABLE [SetPiece] (
  [SetPieceId] int PRIMARY KEY IDENTITY(1, 1),
  [Piece] int,
  [SetId] int,
  [Amount] int,
  [Colour] int
)
GO

CREATE TABLE [PieceCategory] (
  [PieceCategoryId] int PRIMARY KEY IDENTITY(1, 1),
  [PieceCategoryName] VARCHAR
)
GO

CREATE TABLE [Set] (
  [SetId] int PRIMARY KEY IDENTITY(1, 1),
  [SetName] VARCHAR,
  [PiecesAmount] int,
  [SetCategory] int,
  [ShopLink] VARCHAR,
  [Image] VARCHAR
)
GO

CREATE TABLE [SetCategory] (
  [SetCategoryId] int PRIMARY KEY IDENTITY(1, 1),
  [SetCategoryName] VARCHAR
)
GO

CREATE TABLE [User] (
  [UserId] int PRIMARY KEY IDENTITY(1, 1),
  [Username] VARCHAR
)
GO

CREATE TABLE [UserSparePieces] (
  [User] int,
  [Piece] int,
  [Amount] int,
  [Colour] int
)
GO

CREATE TABLE [UserSet] (
  [UseSetId] int PRIMARY KEY IDENTITY(1, 1),
  [User] int,
  [Set] int
)
GO

CREATE TABLE [Colour] (
  [ColourId] int PRIMARY KEY IDENTITY(1, 1),
  [ColourName] VARCHAR
)
GO

CREATE TABLE [MissingPiece] (
  [MissingPieceId] int PRIMARY KEY IDENTITY(1, 1),
  [Piece] int,
  [UserSet] int,
  [Amount] int,
  [Colour] int
)
GO

CREATE TABLE [SetPieceCategory] (
  [SetPieceCategoryId] int PRIMARY KEY IDENTITY(1, 1),
  [PieceCategory] int,
  [Set] int
)
GO

ALTER TABLE [Set] ADD FOREIGN KEY ([SetCategory]) REFERENCES [SetCategory] ([SetCategoryId])
GO

ALTER TABLE [Piece] ADD FOREIGN KEY ([PieceCategory]) REFERENCES [PieceCategory] ([PieceCategoryId])
GO

ALTER TABLE [SetPiece] ADD FOREIGN KEY ([SetPieceId]) REFERENCES [Piece] ([PieceId])
GO

ALTER TABLE [SetPiece] ADD FOREIGN KEY ([SetId]) REFERENCES [Set] ([SetId])
GO

ALTER TABLE [UserSparePieces] ADD FOREIGN KEY ([User]) REFERENCES [User] ([UserId])
GO

ALTER TABLE [UserSparePieces] ADD FOREIGN KEY ([Piece]) REFERENCES [Piece] ([PieceId])
GO

ALTER TABLE [UserSet] ADD FOREIGN KEY ([User]) REFERENCES [User] ([UserId])
GO

ALTER TABLE [UserSet] ADD FOREIGN KEY ([Set]) REFERENCES [Set] ([SetId])
GO

ALTER TABLE [SetPieceCategory] ADD FOREIGN KEY ([PieceCategory]) REFERENCES [PieceCategory] ([PieceCategoryId])
GO

ALTER TABLE [SetPieceCategory] ADD FOREIGN KEY ([Set]) REFERENCES [Set] ([SetId])
GO

ALTER TABLE [MissingPiece] ADD FOREIGN KEY ([UserSet]) REFERENCES [UserSet] ([UseSetId])
GO

ALTER TABLE [MissingPiece] ADD FOREIGN KEY ([Piece]) REFERENCES [Piece] ([PieceId])
GO

ALTER TABLE [SetPiece] ADD FOREIGN KEY ([Colour]) REFERENCES [Colour] ([ColourId])
GO

ALTER TABLE [UserSparePieces] ADD FOREIGN KEY ([Colour]) REFERENCES [Colour] ([ColourId])
GO

ALTER TABLE [MissingPiece] ADD FOREIGN KEY ([Colour]) REFERENCES [Colour] ([ColourId])
GO

ALTER TABLE [SubstitutePoolItem] ADD FOREIGN KEY ([SubstitutePool]) REFERENCES [SubstitutePool] ([SubstitutePool])
GO

ALTER TABLE [SubstitutePoolItem] ADD FOREIGN KEY ([Piece]) REFERENCES [Piece] ([PieceId])
GO

ALTER TABLE [UserSubsititutePool] ADD FOREIGN KEY ([SubstitutePool]) REFERENCES [SubstitutePool] ([SubstitutePool])
GO

ALTER TABLE [UserSubsititutePool] ADD FOREIGN KEY ([User]) REFERENCES [User] ([UserId])
GO
