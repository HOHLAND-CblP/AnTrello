using FluentMigrator;

namespace AnTrello.Backend.DbInfrastructure.Migrations;

[Migration(202407211500, "Init base schema")]
public class AddBaseSchema : Migration
{
    public override void Up()
    {
        string sql =
            """
            CREATE TABLE IF NOT EXISTS users (
                id              bigserial PRIMARY KEY,
                email           varchar unique NOT NULL,
                name            varchar,
                password        varchar NOT NULL,
                break_interval  int default(50),
                intervals_count int default(10),
                work_iterval    int default(7),
                created_at      timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at      timestamp with time zone
            );

            CREATE TABLE IF NOT EXISTS tasks (
                id              bigserial PRIMARY KEY,
                name            varchar NOT NULL,
                is_completed    boolean NOT NUll default(false),
                user_id         bigint NOT NULL references users(id),
                priority        varchar NOT NULL,
                created_at      timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at      timestamp with time zone 
            );

            CREATE TABLE IF NOT EXISTS time_blocks (
                id          bigserial PRIMARY KEY,
                name        varchar NOT NULL,
                color       varchar,
                duration    int NOT NULL,
                "order"     int NOT NULL default(1),
                user_id     bigint NOT NULL references users(id),
                created_at  timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at  timestamp with time zone 
            );

            CREATE TABLE IF NOT EXISTS pomodoro_sessinos (
                id              bigserial PRIMARY KEY,
                total_seconds   bigint NOT NULL,
                is_completed    boolean NOT NUll default(false),
                user_id         bigint NOT NULL references users(id),
                created_at      timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at      timestamp with time zone 
            );

            CREATE TABLE IF NOT EXISTS pomodoro_rounds  (
                id                  bigserial PRIMARY KEY,
                tital_seconds       bigint NOT NULL,
                is_completed        boolean NOT NUll default(false),
                pomodoro_session_id bigint NOT NULL references pomodoro_sessinos(id) ON DELETE CASCADE,
                created_at          timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at          timestamp with time zone 
            );
            """;
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        string sql =
            """
            DROP TABLE pomodoro_rounds;
            DROP TABLE pomodoro_sessinos;
            DROP TABLE time_blocks;
            DROP TABLE tasks;
            DROP TABLE users;
            """;
        
        Execute.Sql(sql);
    }
}