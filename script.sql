IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Tareas] (
    [Id] int NOT NULL IDENTITY,
    [Titulo] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(500) NULL,
    [Completada] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaRealizacion] datetime2 NULL,
    [ImagenNombreArchivo] nvarchar(max) NULL,
    CONSTRAINT [PK_Tareas] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260924223416_InitialCreate', N'10.0.12');

COMMIT;
GO

