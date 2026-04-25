using KateMorrisonMCP.Data;

namespace KateMorrisonMCP.Tests;

/// <summary>
/// Helper methods for test database setup
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Creates the database schema for testing
    /// This is a minimal schema with just the tables needed for tests
    /// </summary>
    public static async Task CreateTestSchemaAsync(DatabaseContext db)
    {
        await db.InitializeAsync();

        // Create characters table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS characters (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                full_name TEXT NOT NULL UNIQUE,
                preferred_name TEXT,
                age INTEGER,
                birthday TEXT,
                birth_year INTEGER,
                height_inches INTEGER,
                weight_lbs INTEGER,
                build TEXT,
                hair_color TEXT,
                hair_length TEXT,
                eye_color TEXT,
                distinctive_features TEXT,
                occupation TEXT,
                employer TEXT,
                job_title TEXT,
                work_location TEXT,
                work_schedule_type TEXT,
                phone TEXT,
                email TEXT,
                residence_id INTEGER,
                character_type TEXT DEFAULT 'secondary',
                is_alive INTEGER DEFAULT 1,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP
            );
        ");

        // Create character_negatives table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS character_negatives (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                character_id INTEGER NOT NULL,
                negative_category TEXT NOT NULL,
                negative_behavior TEXT NOT NULL,
                strength TEXT NOT NULL DEFAULT 'strong',
                explanation TEXT,
                exception_conditions TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (character_id) REFERENCES characters(id)
            );
        ");

        // Create locations table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS locations (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                address_street TEXT,
                address_city TEXT,
                address_state TEXT,
                address_zip TEXT,
                location_type TEXT NOT NULL,
                building_type TEXT,
                floor_count INTEGER,
                description TEXT,
                special_features TEXT,
                access_notes TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP
            );
        ");

        // Create location_rooms table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS location_rooms (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                location_id INTEGER NOT NULL,
                room_name TEXT NOT NULL,
                floor_level TEXT,
                room_type TEXT,
                dimensions_sqft INTEGER,
                description TEXT,
                furnishings TEXT,
                notes TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (location_id) REFERENCES locations(id)
            );
        ");

        // Create schedules table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS schedules (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                character_id INTEGER NOT NULL,
                schedule_type TEXT NOT NULL,
                schedule_name TEXT NOT NULL DEFAULT '',
                days_of_week TEXT,
                start_time TEXT,
                end_time TEXT,
                duration_minutes INTEGER,
                location_id INTEGER,
                location_description TEXT,
                description TEXT,
                exceptions TEXT,
                effective_from TEXT,
                effective_until TEXT,
                is_current INTEGER DEFAULT 1,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (character_id) REFERENCES characters(id),
                FOREIGN KEY (location_id) REFERENCES locations(id)
            );
        ");

        // Create timeline_events table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS timeline_events (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                event_date TEXT NOT NULL,
                event_date_precision TEXT DEFAULT 'day',
                event_title TEXT NOT NULL,
                event_description TEXT,
                location_id INTEGER,
                location_description TEXT,
                event_category TEXT,
                significance TEXT,
                is_canonical INTEGER DEFAULT 1,
                source_file TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (location_id) REFERENCES locations(id)
            );
        ");

        // Create event_participants table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS event_participants (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                event_id INTEGER NOT NULL,
                character_id INTEGER NOT NULL,
                role TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (event_id) REFERENCES timeline_events(id),
                FOREIGN KEY (character_id) REFERENCES characters(id),
                UNIQUE(event_id, character_id)
            );
        ");

        // Create relationships table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS relationships (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                character_a_id INTEGER NOT NULL,
                character_b_id INTEGER NOT NULL,
                relationship_type TEXT NOT NULL,
                relationship_subtype TEXT,
                start_date TEXT,
                end_date TEXT,
                current_status TEXT,
                direction TEXT,
                notes TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (character_a_id) REFERENCES characters(id),
                FOREIGN KEY (character_b_id) REFERENCES characters(id),
                UNIQUE(character_a_id, character_b_id, relationship_type)
            );
        ");

        // Create possessions table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS possessions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                character_id INTEGER NOT NULL,
                item_name TEXT NOT NULL,
                item_type TEXT NOT NULL,
                description TEXT,
                acquisition_date TEXT,
                acquisition_status TEXT,
                acquisition_details TEXT,
                current_location_id INTEGER,
                storage_location TEXT,
                significance TEXT,
                notes TEXT,
                is_current INTEGER DEFAULT 1,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (character_id) REFERENCES characters(id),
                FOREIGN KEY (current_location_id) REFERENCES locations(id)
            );
        ");

        // Create education table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS education (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                character_id INTEGER NOT NULL,
                institution TEXT NOT NULL,
                degree_type TEXT NOT NULL,
                field_of_study TEXT,
                start_year INTEGER,
                end_year INTEGER,
                is_completed INTEGER DEFAULT 0,
                is_current INTEGER DEFAULT 0,
                advisor TEXT,
                notes TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (character_id) REFERENCES characters(id)
            );
        ");

        // Create update_log table
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS update_log (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                update_date TEXT DEFAULT CURRENT_TIMESTAMP,
                table_name TEXT NOT NULL,
                record_id INTEGER,
                change_type TEXT NOT NULL,
                description TEXT,
                source TEXT
            );
        ");
    }
}
