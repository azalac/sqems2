INSERT INTO people(HCN, lastName, firstName, mInitial, dateBirth, sex, HouseID) VALUES('0123456789AB', 'smith', 'bob', 'c', '1997-11-20', 'M', NULL);
INSERT INTO people(HCN, lastName, firstName, mInitial, dateBirth, sex, HouseID) VALUES('0123456789AB', 'smithert', 'bobert', 'c', '1997-11-21', 'M', NULL);

CALL CreateAppointment(2005, 5, 25, 3, 1, 2)
