create table "Tour"
(
	"Id" serial
		constraint tour_pk
			primary key,
	"Name" text not null,
	"Description" text not null,
	"Information" text,
	"DistanceInKm" double precision not null
);
