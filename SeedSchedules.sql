DELETE FROM TourSchedules;
DBCC CHECKIDENT ('TourSchedules', RESEED, 0);

-- Randomly add some schedules for the next 30 days
INSERT INTO TourSchedules (TourId, DepartureDate, Price, MaxParticipants, CurrentParticipants, IsActive)
SELECT Id, DATEADD(day, 3, GETDATE()), 0, 30, ABS(CHECKSUM(NEWID()) % 25), 1 FROM Tours;

INSERT INTO TourSchedules (TourId, DepartureDate, Price, MaxParticipants, CurrentParticipants, IsActive)
SELECT Id, DATEADD(day, 7, GETDATE()), 0, 30, ABS(CHECKSUM(NEWID()) % 25), 1 FROM Tours;

INSERT INTO TourSchedules (TourId, DepartureDate, Price, MaxParticipants, CurrentParticipants, IsActive)
SELECT Id, DATEADD(day, 12, GETDATE()), Price + 500000, 30, ABS(CHECKSUM(NEWID()) % 25), 1 FROM Tours;

INSERT INTO TourSchedules (TourId, DepartureDate, Price, MaxParticipants, CurrentParticipants, IsActive)
SELECT Id, DATEADD(day, 17, GETDATE()), 0, 30, ABS(CHECKSUM(NEWID()) % 25), 1 FROM Tours;

INSERT INTO TourSchedules (TourId, DepartureDate, Price, MaxParticipants, CurrentParticipants, IsActive)
SELECT Id, DATEADD(day, 24, GETDATE()), Price - 200000, 30, ABS(CHECKSUM(NEWID()) % 25), 1 FROM Tours;

PRINT 'Seeded TourSchedules successfully!';
