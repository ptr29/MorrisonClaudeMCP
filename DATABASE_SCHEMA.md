# Database Schema

SQLite database: `data/canonical_facts.db`

---

## `characters`

Core table for all fictional characters.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `full_name` | TEXT | NOT NULL, UNIQUE | |
| `preferred_name` | TEXT | | Name used for lookups (e.g. "Kate", not "Kate Morrison") |
| `age` | INTEGER | | |
| `birthday` | TEXT | | |
| `birth_year` | INTEGER | | |
| `height_inches` | INTEGER | | |
| `weight_lbs` | INTEGER | | |
| `build` | TEXT | | |
| `hair_color` | TEXT | | |
| `hair_length` | TEXT | | |
| `eye_color` | TEXT | | |
| `distinctive_features` | TEXT | | |
| `occupation` | TEXT | | |
| `employer` | TEXT | | |
| `job_title` | TEXT | | |
| `work_location` | TEXT | | |
| `work_schedule_type` | TEXT | | |
| `phone` | TEXT | | |
| `email` | TEXT | | |
| `residence_id` | INTEGER | FK → `locations.id` | |
| `character_type` | TEXT | DEFAULT `'secondary'` | |
| `is_alive` | INTEGER | DEFAULT `1` | Boolean (0/1) |
| `created_at` | TEXT | DEFAULT CURRENT_TIMESTAMP | |
| `updated_at` | TEXT | DEFAULT CURRENT_TIMESTAMP | |
| `source_file` | TEXT | | Source data file |

**Indexes:** `(full_name, preferred_name)`

---

## `locations`

Physical locations (residences, workplaces, etc.).

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `name` | TEXT | NOT NULL | |
| `address_street` | TEXT | | |
| `address_city` | TEXT | | |
| `address_state` | TEXT | | |
| `address_zip` | TEXT | | |
| `location_type` | TEXT | NOT NULL | |
| `building_type` | TEXT | | |
| `floor_count` | INTEGER | | |
| `unit_number` | TEXT | | |
| `square_feet` | INTEGER | | |
| `owner_id` | INTEGER | FK → `characters.id` | |
| `ownership_type` | TEXT | | e.g. rent, own |
| `monthly_cost` | REAL | | |
| `purchase_date` | TEXT | | |
| `neighborhood` | TEXT | | |
| `distance_to_chicago_loop` | TEXT | | |
| `nearby_landmarks` | TEXT | | |
| `is_fictional` | INTEGER | DEFAULT `0` | Boolean (0/1) |
| `created_at` | TEXT | DEFAULT CURRENT_TIMESTAMP | |
| `updated_at` | TEXT | DEFAULT CURRENT_TIMESTAMP | |
| `source_file` | TEXT | | |

**Indexes:** `(name)`, `(address_city, address_street)`

---

## `location_rooms`

Individual rooms within a location.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `location_id` | INTEGER | NOT NULL, FK → `locations.id` | |
| `room_name` | TEXT | NOT NULL | |
| `floor_level` | TEXT | NOT NULL | e.g. "1", "basement" |
| `room_type` | TEXT | NOT NULL | e.g. bedroom, kitchen |
| `width_feet` | REAL | | |
| `length_feet` | REAL | | |
| `width_inches` | REAL | | |
| `length_inches` | REAL | | |
| `current_use` | TEXT | | |
| `key_features` | TEXT | | |
| `furniture` | TEXT | | |
| `adjacent_to` | TEXT | | |
| `has_exterior_window` | INTEGER | DEFAULT `0` | Boolean (0/1) |
| `window_direction` | TEXT | | e.g. north, south |
| `source_file` | TEXT | | |

**Indexes:** `(location_id)`, `(location_id, floor_level)`

---

## `timeline_events`

Canonical events in the story timeline.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `event_date` | TEXT | NOT NULL | |
| `event_date_precision` | TEXT | DEFAULT `'day'` | e.g. day, month, year |
| `event_title` | TEXT | NOT NULL | |
| `event_description` | TEXT | | |
| `location_id` | INTEGER | FK → `locations.id` | |
| `location_description` | TEXT | | Free-text fallback if no location_id |
| `event_category` | TEXT | | |
| `significance` | TEXT | | |
| `is_canonical` | INTEGER | DEFAULT `1` | Boolean (0/1) |
| `source_file` | TEXT | | |
| `created_at` | TEXT | DEFAULT CURRENT_TIMESTAMP | |

**Indexes:** `(event_date)`, `(event_category)`

---

## `event_participants`

Join table linking characters to timeline events.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `event_id` | INTEGER | NOT NULL, FK → `timeline_events.id` | |
| `character_id` | INTEGER | NOT NULL, FK → `characters.id` | |
| `role` | TEXT | | Character's role in the event |

**Unique:** `(event_id, character_id)`  
**Indexes:** `(event_id)`, `(character_id)`

---

## `relationships`

Relationships between pairs of characters.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `character_a_id` | INTEGER | NOT NULL, FK → `characters.id` | |
| `character_b_id` | INTEGER | NOT NULL, FK → `characters.id` | |
| `relationship_type` | TEXT | NOT NULL | e.g. romantic, friend, family |
| `relationship_subtype` | TEXT | | e.g. boyfriend/girlfriend |
| `start_date` | TEXT | | |
| `end_date` | TEXT | | |
| `current_status` | TEXT | | |
| `direction` | TEXT | | bidirectional or directional |
| `notes` | TEXT | | |
| `source_file` | TEXT | | |

**Unique:** `(character_a_id, character_b_id, relationship_type)`  
**Indexes:** `(character_a_id, character_b_id)`

---

## `possessions`

Items owned by characters.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `owner_id` | INTEGER | NOT NULL, FK → `characters.id` | |
| `item_name` | TEXT | NOT NULL | |
| `item_category` | TEXT | | |
| `item_description` | TEXT | | |
| `acquisition_date` | TEXT | | |
| `acquisition_method` | TEXT | | e.g. purchased, gifted |
| `acquisition_from` | TEXT | | |
| `location_id` | INTEGER | FK → `locations.id` | Where item is kept |
| `storage_location` | TEXT | | Free-text storage detail |
| `monetary_value` | REAL | | |
| `sentimental_value` | TEXT | | |
| `significance_notes` | TEXT | | |
| `is_current` | INTEGER | DEFAULT `1` | Boolean (0/1) — still owned? |
| `source_file` | TEXT | | |

**Indexes:** `(owner_id)`, `(item_category)`

---

## `schedules`

Recurring schedule entries for characters.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `character_id` | INTEGER | NOT NULL, FK → `characters.id` | |
| `schedule_type` | TEXT | NOT NULL | e.g. work, exercise, social |
| `schedule_name` | TEXT | NOT NULL | |
| `days_of_week` | TEXT | | e.g. "Mon,Tue,Wed" |
| `start_time` | TEXT | | HH:MM |
| `end_time` | TEXT | | HH:MM |
| `duration_minutes` | INTEGER | | |
| `location_id` | INTEGER | FK → `locations.id` | |
| `location_description` | TEXT | | Free-text fallback |
| `description` | TEXT | | |
| `exceptions` | TEXT | | Notes on exceptions |
| `effective_from` | TEXT | | |
| `effective_until` | TEXT | | |
| `is_current` | INTEGER | DEFAULT `1` | Boolean (0/1) |
| `source_file` | TEXT | | |

**Indexes:** `(character_id)`, `(schedule_type)`

---

## `character_negatives`

Behaviors a character explicitly does NOT do.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `character_id` | INTEGER | NOT NULL, FK → `characters.id` | |
| `negative_category` | TEXT | NOT NULL | e.g. diet, social, habits |
| `negative_behavior` | TEXT | NOT NULL | Description of what they don't do |
| `strength` | TEXT | DEFAULT `'strong'` | e.g. strong, moderate |
| `explanation` | TEXT | | Why they don't do it |
| `exception_conditions` | TEXT | | Any edge-case exceptions |
| `source_file` | TEXT | | |

**Indexes:** `(character_id)`, `(negative_category)`

---

## `education`

Educational history for characters.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `character_id` | INTEGER | NOT NULL, FK → `characters.id` | |
| `institution` | TEXT | NOT NULL | |
| `degree_type` | TEXT | | e.g. BS, MS, PhD |
| `field_of_study` | TEXT | | |
| `start_year` | INTEGER | | |
| `end_year` | INTEGER | | |
| `is_completed` | INTEGER | DEFAULT `1` | Boolean (0/1) |
| `is_current` | INTEGER | DEFAULT `0` | Boolean (0/1) |
| `honors` | TEXT | | |
| `thesis_title` | TEXT | | |
| `advisor` | TEXT | | |
| `notes` | TEXT | | |
| `source_file` | TEXT | | |

**Indexes:** `(character_id)`

---

## `update_log`

Audit trail of field-level changes.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `entity_type` | TEXT | NOT NULL | e.g. "character", "location" |
| `entity_id` | INTEGER | NOT NULL | ID in the referenced table |
| `field_name` | TEXT | NOT NULL | Column that changed |
| `old_value` | TEXT | | |
| `new_value` | TEXT | | |
| `source` | TEXT | | |
| `updated_at` | TEXT | DEFAULT CURRENT_TIMESTAMP | |

**Indexes:** `(entity_type, entity_id)`

---

## `ingestion_metadata`

Run statistics for each ingestion execution.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, AUTOINCREMENT | |
| `run_timestamp` | TEXT | NOT NULL | |
| `mode` | TEXT | NOT NULL | e.g. full, incremental |
| `source_directory` | TEXT | NOT NULL | |
| `files_processed` | INTEGER | DEFAULT `0` | |
| `tags_processed` | INTEGER | DEFAULT `0` | |
| `records_inserted` | INTEGER | DEFAULT `0` | |
| `records_updated` | INTEGER | DEFAULT `0` | |
| `records_deleted` | INTEGER | DEFAULT `0` | |
| `errors` | INTEGER | DEFAULT `0` | |
| `duration_seconds` | REAL | | |

---

## `last_run_state`

Singleton row tracking the last successful ingestion run.

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `id` | INTEGER | PK, DEFAULT `1` | Always 1 (enforced by CHECK) |
| `last_successful_run` | TEXT | | Timestamp of last clean run |

**Check constraint:** `id = 1`

---

## Entity Relationship Summary

```
characters ──< education
characters ──< schedules
characters ──< possessions
characters ──< character_negatives
characters ──< event_participants >── timeline_events
characters >── relationships ──< characters
characters ──> locations (residence_id)
locations ──< location_rooms
locations ──< timeline_events
locations ──< possessions
locations ──< schedules
```
