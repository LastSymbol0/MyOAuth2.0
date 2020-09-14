dotnet testx ^
	--project all ^
	--html true --browser true ^
	--opencover-filters "+[AuthServer*]* -[AuthServer.UnitTests]* -[AuthServer.Views]* -[AuthServer.Infrastructure]*Migrations* -[AuthServer]*Startup* -[AuthServer]*Program*"