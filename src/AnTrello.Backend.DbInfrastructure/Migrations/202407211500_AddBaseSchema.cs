using FluentMigrator;
using FluentMigrator.Expressions;

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
                break_interval  int default(7),
                intervals_count int default(10),
                work_interval   int default(50),
                created_at      timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at      timestamp with time zone
            );

            CREATE TABLE IF NOT EXISTS jwt_refresh_tokens (
                token           varchar PRIMARY KEY,
                user_id         bigint NOT NULL references users(id) ON DELETE CASCADE,
                is_activated    boolean NOT NUll DEFAULT(false)
            );

            CREATE TABLE IF NOT EXISTS tasks (
                id              bigserial PRIMARY KEY,
                name            varchar NOT NULL,
                is_completed    boolean NOT NUll default(false),
                user_id         bigint NOT NULL references users(id) ON DELETE CASCADE,
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
                user_id     bigint NOT NULL references users(id) ON DELETE CASCADE,
                created_at  timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at  timestamp with time zone 
            );

            CREATE TABLE IF NOT EXISTS pomodoro_sessions (
                id              bigserial PRIMARY KEY,
                total_seconds   bigint NOT NULL,
                is_completed    boolean NOT NUll default(false),
                user_id         bigint NOT NULL references users(id) ON DELETE CASCADE,
                created_at      timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at      timestamp with time zone 
            );

            CREATE TABLE IF NOT EXISTS pomodoro_rounds  (
                id                  bigserial PRIMARY KEY,
                total_seconds       bigint NOT NULL,
                is_completed        boolean NOT NUll default(false),
                pomodoro_session_id bigint NOT NULL references pomodoro_sessions(id) ON DELETE CASCADE,
                created_at          timestamp with time zone NOT NULL default (now() at time zone 'utc' ),
                updated_at          timestamp with time zone
            );  

            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'pomodoro_round_v1') THEN
                    CREATE TYPE pomodoro_round_v1 as
                    (
                        id                  bigint,
                        total_seconds       bigint,
                        is_completed        boolean,
                        pomodoro_session_id bigint,
                        created_at          timestamp with time zone,
                        updated_at          timestamp with time zone
                    );
                END IF;
            END
            $$;
            """;
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        string sql =
            """
            DO $$
                BEGIN
                    DROP TYPE IF EXISTS pomodoro_round_v1;
                END
            $$;
            
            DROP TABLE pomodoro_rounds;
            DROP TABLE pomodoro_sessions;
            DROP TABLE time_blocks;
            DROP TABLE tasks;
            DROP TABLE jwt_refresh_tokens;
            DROP TABLE users;
            """;
        
        Execute.Sql(sql);
    }
}