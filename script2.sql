BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tareas]') AND [c].[name] = N'ImagenNombreArchivo');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Tareas] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Tareas] DROP COLUMN [ImagenNombreArchivo];

ALTER TABLE [Tareas] ADD [ImagenContentType] nvarchar(100) NULL;

ALTER TABLE [Tareas] ADD [ImagenDatos] varbinary(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260926222009_AgregarImagenComoBytes', N'10.0.12');

COMMIT;
GO

