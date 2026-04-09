dotnet publish -c Release -o C:\inetpub\ollama



-- Habilitar Mixed Mode
EXEC xp_instance_regwrite 
  N'HKEY_LOCAL_MACHINE', 
  N'Software\Microsoft\MSSQLServer\MSSQLServer',
  N'LoginMode', 
  REG_DWORD, 
  2;

-- Habilitar o login sa
ALTER LOGIN sa ENABLE;

-- Definir a senha
ALTER LOGIN sa WITH PASSWORD = 'SenhaForte123!';


 
