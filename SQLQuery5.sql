Exec sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'
Exec sp_MSforeachtable @command1 = 'DROP TABLE ?'