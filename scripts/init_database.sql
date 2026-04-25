-- Complete database initialization script for MCP Canonical Facts Server
-- Kate Morrison Creative Writing Project
-- Schema version: 1.0.0

PRAGMA journal_mode=WAL;
PRAGMA foreign_keys=ON;

-- Characters
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
    updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (residence_id) REFERENCES locations(id)
);

CREATE INDEX IF NOT EXISTS idx_characters_name ON characters(full_name, preferred_name);

-- Locations
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
    unit_number TEXT,
    square_feet INTEGER,
    owner_id INTEGER,
    ownership_type TEXT,
    monthly_cost REAL,
    purchase_date TEXT,
    neighborhood TEXT,
    distance_to_chicago_loop TEXT,
    nearby_landmarks TEXT,
    is_fictional INTEGER DEFAULT 0,
    created_at TEXT DEFAULT CURRENT_TIMESTAMP,
    updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (owner_id) REFERENCES characters(id)
);

CREATE INDEX IF NOT EXISTS idx_locations_name ON locations(name);
CREATE INDEX IF NOT EXISTS idx_locations_address ON locations(address_city, address_street);

-- Location Rooms
CREATE TABLE IF NOT EXISTS location_rooms (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    location_id INTEGER NOT NULL,
    room_name TEXT NOT NULL,
    floor_level TEXT NOT NULL,
    room_type TEXT NOT NULL,
    width_feet REAL,
    length_feet REAL,
    width_inches REAL,
    length_inches REAL,
    current_use TEXT,
    key_features TEXT,
    furniture TEXT,
    adjacent_to TEXT,
    has_exterior_window INTEGER DEFAULT 0,
    window_direction TEXT,
    FOREIGN KEY (location_id) REFERENCES locations(id)
);

CREATE INDEX IF NOT EXISTS idx_rooms_location ON location_rooms(location_id);
CREATE INDEX IF NOT EXISTS idx_rooms_floor ON location_rooms(location_id, floor_level);

-- Timeline Events
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

CREATE INDEX IF NOT EXISTS idx_timeline_date ON timeline_events(event_date);
CREATE INDEX IF NOT EXISTS idx_timeline_category ON timeline_events(event_category);

-- Event Participants
CREATE TABLE IF NOT EXISTS event_participants (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_id INTEGER NOT NULL,
    character_id INTEGER NOT NULL,
    role TEXT,
    FOREIGN KEY (event_id) REFERENCES timeline_events(id),
    FOREIGN KEY (character_id) REFERENCES characters(id),
    UNIQUE(event_id, character_id)
);

CREATE INDEX IF NOT EXISTS idx_event_participants ON event_participants(event_id);
CREATE INDEX IF NOT EXISTS idx_character_events ON event_participants(character_id);

-- Relationships
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
    FOREIGN KEY (character_a_id) REFERENCES characters(id),
    FOREIGN KEY (character_b_id) REFERENCES characters(id),
    UNIQUE(character_a_id, character_b_id, relationship_type)
);

CREATE INDEX IF NOT EXISTS idx_relationships_chars ON relationships(character_a_id, character_b_id);

-- Possessions
CREATE TABLE IF NOT EXISTS possessions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    owner_id INTEGER NOT NULL,
    item_name TEXT NOT NULL,
    item_category TEXT,
    item_description TEXT,
    acquisition_date TEXT,
    acquisition_method TEXT,
    acquisition_from TEXT,
    location_id INTEGER,
    storage_location TEXT,
    monetary_value REAL,
    sentimental_value TEXT,
    significance_notes TEXT,
    is_current INTEGER DEFAULT 1,
    FOREIGN KEY (owner_id) REFERENCES characters(id),
    FOREIGN KEY (location_id) REFERENCES locations(id)
);

CREATE INDEX IF NOT EXISTS idx_possessions_owner ON possessions(owner_id);
CREATE INDEX IF NOT EXISTS idx_possessions_category ON possessions(item_category);

-- Schedules
CREATE TABLE IF NOT EXISTS schedules (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    schedule_type TEXT NOT NULL,
    schedule_name TEXT NOT NULL,
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
    FOREIGN KEY (character_id) REFERENCES characters(id),
    FOREIGN KEY (location_id) REFERENCES locations(id)
);

CREATE INDEX IF NOT EXISTS idx_schedules_character ON schedules(character_id);
CREATE INDEX IF NOT EXISTS idx_schedules_type ON schedules(schedule_type);

-- Character Negatives (CRITICAL FOR ERROR PREVENTION)
CREATE TABLE IF NOT EXISTS character_negatives (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    negative_category TEXT NOT NULL,
    negative_behavior TEXT NOT NULL,
    strength TEXT DEFAULT 'strong',
    explanation TEXT,
    exception_conditions TEXT,
    FOREIGN KEY (character_id) REFERENCES characters(id)
);

CREATE INDEX IF NOT EXISTS idx_negatives_character ON character_negatives(character_id);
CREATE INDEX IF NOT EXISTS idx_negatives_category ON character_negatives(negative_category);

-- Education
CREATE TABLE IF NOT EXISTS education (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    institution TEXT NOT NULL,
    degree_type TEXT,
    field_of_study TEXT,
    start_year INTEGER,
    end_year INTEGER,
    is_completed INTEGER DEFAULT 1,
    is_current INTEGER DEFAULT 0,
    honors TEXT,
    thesis_title TEXT,
    advisor TEXT,
    notes TEXT,
    FOREIGN KEY (character_id) REFERENCES characters(id)
);

CREATE INDEX IF NOT EXISTS idx_education_character ON education(character_id);

-- Update Log (for tracking changes)
CREATE TABLE IF NOT EXISTS update_log (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    entity_type TEXT NOT NULL,
    entity_id INTEGER NOT NULL,
    field_name TEXT NOT NULL,
    old_value TEXT,
    new_value TEXT,
    source TEXT,
    updated_at TEXT DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_update_log_entity ON update_log(entity_type, entity_id);
