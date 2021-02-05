# Cwiczenie5
Weronika Jaworek, s18630


Procedura :
USE [2019SBD]
GO
/****** Object:  StoredProcedure [s18630].[PromoteStudents]    Script Date: 05.02.2021 11:23:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [s18630].[PromoteStudents] @Studies NVARCHAR(100), @Semester INT
AS
BEGIN
	SET XACT_ABORT ON;
	BEGIN TRAN

	DECLARE @IdStudy INT= (SELECT IdStudy FROM Studies WHERE Name=@Studies);
		IF @IdStudy IS NULL
			BEGIN

				RAISERROR ('Studia nie istnieją', 16,  1);
				RETURN;
			END

	DECLARE @IdEnrollment INT= (SELECT Max(IdEnrollment) FROM  Enrollment WHERE IdStudy=@IdStudy AND Semester=@Semester);
		IF @IdEnrollment IS NULL
			BEGIN
				 RAISERROR ('Semestr nie istnieje', 16,  1);
				RETURN;
			END


		DECLARE @NewSemester INT=@Semester+1;

		DECLARE @NewIdEnrollment INT=(SELECT Max(IdEnrollment) from Enrollment where Semester=@NewSemester and IdStudy=@IdStudy);
			IF @NewIdEnrollment IS NULL
			BEGIN
				INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES
				 ((SELECT MAX(IdEnrollment) from Enrollment )+1 , @NewSemester, @IdStudy,  convert(datetime, GETDATE ()));

				SET  @NewIdEnrollment =(SELECT IdEnrollment from Enrollment where IdEnrollment = (SELECT MAX(IdEnrollment) from Enrollment));
				
			
			END


			
		UPDATE Student SET IdEnrollment=@NewIdEnrollment WHERE IdEnrollment=@IdEnrollment;

		

	COMMIT
	END;

