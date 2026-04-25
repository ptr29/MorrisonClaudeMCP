-- Sample data for testing MCP Canonical Facts Server
-- Critical characters: Kate Morrison and Paul Rogala
-- Critical facts: Kate's negatives, Paul's 7 AM schedule, Paul's house

-- Insert Kate Morrison
INSERT INTO characters (
    full_name, preferred_name, age, birthday, height_inches, weight_lbs,
    hair_color, hair_length, eye_color, build, distinctive_features,
    occupation, employer, character_type
) VALUES (
    'Katherine Marie Morrison', 'Kate', 31, '1993-03-15', 66, 128,
    'blonde', 'shoulder-length', 'green', 'slim',
    '["shoulder-length hair", "runner build", "engaging smile"]',
    'Research Scientist', 'Northwestern University', 'primary'
);

-- Insert Paul Rogala
INSERT INTO characters (
    full_name, preferred_name, age, birthday, height_inches,
    hair_color, eye_color, build, distinctive_features,
    occupation, employer, job_title, character_type
) VALUES (
    'Paul Rogala', 'Paul', 34, '1990-07-12', 70,
    'dark brown', 'brown', 'average',
    '["tech professional appearance", "Mini Cooper enthusiast"]',
    'Software Engineering Manager', 'Confluent', 'Engineering Manager', 'primary'
);

-- Insert Kate's CRITICAL negatives (highest priority for error prevention)
INSERT INTO character_negatives (
    character_id, negative_category, negative_behavior,
    strength, explanation, exception_conditions
) VALUES
(1, 'exercise', 'Does NOT go to gyms', 'absolute',
 'Kate strongly prefers outdoor running and never uses gym facilities',
 NULL),

(1, 'exercise', 'Does NOT run on treadmills', 'absolute',
 'Kate hates treadmills and finds them unbearable',
 '["will use treadmill ONLY if temperature is below 15°F (-9°C)"]'),

(1, 'food', 'Does NOT eat gluten', 'absolute',
 'Kate has celiac disease and must avoid all gluten',
 NULL),

(1, 'food', 'Does NOT drink regular coffee after 2 PM', 'strong',
 'Kate avoids caffeine in afternoon to protect sleep quality',
 '["may have decaf after 2 PM", "exceptions for special occasions"]'),

(1, 'exercise', 'Does NOT do yoga', 'preference',
 'Kate tried yoga but finds it boring and prefers running',
 NULL);

-- Insert Paul's CRITICAL work schedule (7 AM start - most common error)
INSERT INTO schedules (
    character_id, schedule_type, schedule_name,
    days_of_week, start_time, end_time,
    description, location_description, is_current, exceptions
) VALUES
(2, 'work', 'Mumbai Team Standup',
 '["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]',
 '07:00', '09:00',
 'Early morning Mumbai team standup calls - STARTS AT 7 AM, NOT 9 AM',
 'Home office', 1,
 '["US holidays", "personal time off"]'),

(2, 'work', 'Regular Work Hours',
 '["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]',
 '09:00', '17:00',
 'Regular work hours - meetings, management tasks, occasional coding (5% coding, 95% meetings)',
 'Home office', 1,
 '["US holidays", "personal time off"]'),

(2, 'personal', 'Morning Routine',
 '["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]',
 '05:40', '06:30',
 'Morning routine - wake, breakfast, coffee',
 'Home', 1, NULL);

-- Insert Paul's house location
INSERT INTO locations (
    name, location_type, address_street, address_city, address_state, address_zip,
    building_type, floor_count, owner_id, ownership_type,
    neighborhood, nearby_landmarks
) VALUES
('Paul''s House', 'residence', '9636 N Kostner Ave', 'Skokie', 'IL', '60076',
 'Mid-century brick home', 2, 2, 'owned',
 'Skokie',
 '["Skokie Swift CTA station", "I-94 Edens Expressway", "Old Orchard Shopping Center"]');

-- Get location ID (assuming Paul's house = 1)
-- Insert key rooms from Paul's house (main floor + lower level)

-- Main Floor Rooms
INSERT INTO location_rooms (
    location_id, room_name, floor_level, room_type,
    width_feet, length_feet, width_inches, length_inches,
    current_use, furniture, key_features
) VALUES
(1, 'Entry & Foyer', 'Main Floor', 'entry',
 4, 14, 4, 8,
 'Entry foyer',
 '["console table", "coat storage"]',
 '["covered front entry", "long narrow space", "stair run to lower level"]'),

(1, 'Living Room', 'Main Floor', 'living',
 14, 14, 11, 8,
 'Living room',
 '["sofa", "chairs", "32-inch TV", "sound bar", "media furniture", "hutch"]',
 '["curved bay window", "excellent natural light", "near-square proportions"]'),

(1, 'Dining Room', 'Main Floor', 'dining',
 8, 11, 0, 4,
 'Dining room',
 '["6-person dining table"]',
 '["connector between living room and kitchen"]'),

(1, 'Kitchen', 'Main Floor', 'kitchen',
 10, 10, 6, 10,
 'Kitchen',
 '["U-shaped counters", "cabinets"]',
 '["compact but efficient", "side door to patio", "supports serious cooking"]'),

(1, 'Primary Bedroom', 'Main Floor', 'bedroom',
 14, 10, 2, 11,
 'Primary bedroom',
 '["queen bed", "nightstands", "dressers"]',
 '["away from living areas", "true retreat positioning"]'),

(1, 'Bedroom 2', 'Main Floor', 'bedroom',
 11, 11, 1, 3,
 'Guest bedroom',
 '["guest bed", "furniture"]',
 '["notably square", "usable space"]'),

(1, 'Bedroom 3', 'Main Floor', 'office',
 11, 11, 1, 3,
 'Home office',
 '["desk", "two 32-inch monitors on arms", "multiple computers", "filing cabinet"]',
 '["Paul''s primary workspace", "notably square", "professional setup"]'),

(1, 'Full Bath #1', 'Main Floor', 'bathroom',
 7, 5, 3, 2,
 'Full bathroom',
 '["tub/shower", "toilet", "sink"]',
 '["serves bedrooms and guests"]'),

(1, 'Full Bath #2', 'Main Floor', 'bathroom',
 7, 4, 3, 10,
 'Full bathroom',
 '["bathtub", "sink", "toilet"]',
 '["dual-bath configuration", "allows simultaneous use"]');

-- Lower Level Rooms
INSERT INTO location_rooms (
    location_id, room_name, floor_level, room_type,
    width_feet, length_feet, width_inches, length_inches,
    current_use, furniture, key_features
) VALUES
(1, 'Recreation Room', 'Lower Level', 'recreation',
 31, 17, 11, 5,
 'Media and entertainment',
 '["large TV", "sofas", "surround sound", "bookcases"]',
 '["exceptionally large", "standout feature", "long open footprint", "can be zoned"]'),

(1, 'Lower-Level Bedroom', 'Lower Level', 'bedroom',
 11, 15, 7, 9,
 'Guest bedroom / Kate''s workspace',
 '["bed", "desk area"]',
 '["exceeds many main-level bedrooms", "private home office potential"]'),

(1, 'Lower-Level Bathroom', 'Lower Level', 'bathroom',
 6, 7, 5, 3,
 'Full bathroom',
 '["full bath fixtures"]',
 '["enables independent use of lower level"]'),

(1, 'Laundry Room', 'Lower Level', 'utility',
 19, 10, 0, 0,
 'Laundry and utility',
 '["full-size washer", "dryer", "folding stations", "storage cabinets", "utility sink"]',
 '["unusually large", "rare and valuable", "feels like workroom not closet"]'),

(1, 'Mechanical Room', 'Lower Level', 'utility',
 10, 9, 4, 3,
 'Mechanical systems',
 '["HVAC", "water heater"]',
 '["cleanly separated", "doesn''t intrude on living space"]');

-- Insert initial timeline event (Kate and Paul first meeting)
INSERT INTO timeline_events (
    event_date, event_title, event_description,
    event_category, location_description, source_file
) VALUES
('2025-09-06', 'Kate and Paul First Meeting',
 'Kate Morrison and Paul Rogala meet for the first time at a tech industry mixer in Chicago',
 'relationship', 'Chicago tech mixer venue', 'kate-morrison-meeting-paul.md');

-- Link Kate and Paul to the first meeting event (assuming event_id = 1)
INSERT INTO event_participants (event_id, character_id, role)
VALUES
(1, 1, 'participant'),
(1, 2, 'participant');

-- Insert Kate and Paul relationship
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type,
    current_status, start_date, notes
) VALUES
(1, 2, 'romantic', 'dating', '2025-09-06',
 'Kate and Paul began dating after meeting at a tech industry event');

-- Insert Kate's possession (grandmother's necklace)
INSERT INTO possessions (
    owner_id, item_name, item_category, item_description,
    sentimental_value, significance_notes
) VALUES
(1, 'Grandmother''s Necklace', 'jewelry',
 'Silver necklace inherited from Kate''s grandmother',
 'high', 'Highly sentimental family heirloom');
