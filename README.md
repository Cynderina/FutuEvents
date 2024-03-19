# FutuEvents
Preassignment Futurice Junior Software Developer position.

Created API for handling events. With this you can add events, list them, vote for suggested dates and then get results for the votes and as a result you'll get only the dates that'll fit all the votes. As a requirement every voter can vote only once and votes must include only dates that are suggested for the event. 

Software was done using C# and Entity Framework.

The database was created for local PostgreSql, with following SQL:

CREATE TABLE futu_event (
	id SERIAL PRIMARY KEY,
	name VARCHAR(255) NOT NULL
);

CREATE TABLE futu_event_vote (
	id SERIAL PRIMARY KEY,
	event_id int NOT NULL,
	name VARCHAR(255) NOT NULL,
	CONSTRAINT fk_futu_event
		FOREIGN KEY(event_id)
			REFERENCES futu_event(id)
);

CREATE TABLE possible_date (
    id SERIAL PRIMARY KEY,
    event_id INT NOT NULL,
    suggested_date DATE NOT NULL,
    CONSTRAINT fk_futu_event
        FOREIGN KEY(event_id)
            REFERENCES futu_event(id)
)

CREATE TABLE voted_day (
    id SERIAL PRIMARY KEY,
    event_id INT NOT NULL,
    date_id INT NOT NULL,
    vote_id INT NOT NULL,
    CONSTRAINT fk_futu_event
        FOREIGN KEY(event_id)
            REFERENCES futu_event(id),
    CONSTRAINT fk_possible_date
        FOREIGN KEY(date_id)
            REFERENCES possible_date(id),
    CONSTRAINT fk_futu_event_vote
        FOREIGN KEY(vote_id)
            REFERENCES futu_event_vote(id)
)
 
Also for make this working an appsettings file is require "appsettings.json"
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<ServerName>;Port=<Port>;Database=<DbName>;Username=<Username>;Password=<Password>;"
  },
  "Schema": {
    "YourDataSchema": "<SchemaName>"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
