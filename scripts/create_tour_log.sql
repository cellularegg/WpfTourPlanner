create table "TourLog"
(
	"Id" serial
		constraint tourlog_pk
			primary key,
	"Report" text not null,
	"LogDateTime" text not null,
	"TotalTimeInH" double precision not null,
	"Rating" int not null,
	"HeartRate" double precision not null,
	"AverageSpeedInKmH" double precision not null,
	"TemperatureInC" double precision not null,
	"Breaks" int not null,
	"Steps" int not null,
	"TourId" int not null
		constraint tourlog_tour_id_fk
			references "Tour"
				on update cascade on delete cascade
);

