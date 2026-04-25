-- ============================================================================
-- Morrison Claude MCP - Database Update Script (CORRECTED)
-- ============================================================================
-- This script adds missing data identified from project files
-- Generated: 2026-01-23
--
-- Changes Summary:
--   - 5 New Characters (Linda, Priya, Raj, Neha, Prof. Sarah Chen)
--   - 3 New Locations (Kate's apartment, Linda's house, Argonne)
--   - 14 New Timeline Events (Kate-Paul relationship milestones)
--   - 10 New Relationships (family, friendship, professional)
--   - 1 Update (Kate's age: 31 -> 32)
--   - 3 New Schedules (Kate's routines)
--   - 2 New Character Negatives
-- ============================================================================

-- Disable foreign key checks during import
PRAGMA foreign_keys = OFF;

BEGIN TRANSACTION;

-- ============================================================================
-- UPDATE EXISTING RECORDS
-- ============================================================================

-- Kate's age should be 32 (born March 15, 1993, narrative is Dec 2025/Jan 2026)
UPDATE characters SET age = 32 WHERE id = 1;

-- ============================================================================
-- NEW CHARACTERS (5 records)
-- ============================================================================

-- Linda Morrison (Kate's mother) - CRITICAL missing character
INSERT INTO characters (
    full_name, preferred_name, age, birthday,
    height_inches, build, hair_color, hair_length, eye_color,
    distinctive_features, occupation, employer, job_title,
    character_type, is_alive, created_at, updated_at
) VALUES (
    'Linda Marie Morrison', 'Linda', 61, '1964-04-22',
    64, 'soft', 'ash blonde', 'shoulder-length, colored', 'green',
    '["smile lines", "worry lines", "reading glasses on chain", "similar features to Kate"]',
    'Elementary School Teacher (Retired)', 'Naperville School District (Retired)', '4th Grade Teacher (30 years)',
    'secondary', 1, datetime('now'), datetime('now')
);

-- Priya Mehta (Kate's best friend) - CRITICAL missing character
INSERT INTO characters (
    full_name, preferred_name, age, birthday,
    height_inches, build, hair_color, hair_length, eye_color,
    distinctive_features, occupation, employer, job_title,
    character_type, is_alive, created_at, updated_at
) VALUES (
    'Priya Anjali Mehta', 'Priya', 28, '1997-01-01',
    64, 'athletic', 'black', 'long, thick', 'brown',
    '["warm smile", "animated expressions", "talks with hands"]',
    'Postdoctoral Research Associate', 'Argonne National Laboratory', 'Postdoctoral Researcher',
    'secondary', 1, datetime('now'), datetime('now')
);

-- Raj Patel (Priya's fiancé)
INSERT INTO characters (
    full_name, preferred_name, age, birthday,
    height_inches, build, hair_color, hair_length, eye_color,
    distinctive_features, occupation, employer, job_title,
    character_type, is_alive, created_at, updated_at
) VALUES (
    'Rajesh Patel', 'Raj', 30, '1995-01-01',
    70, 'lean', 'black', 'short', 'brown',
    '["calm demeanor", "good listener"]',
    'Software Engineer', 'Trading Firm', 'Software Engineer',
    'supporting', 1, datetime('now'), datetime('now')
);

-- Neha Mehta (Priya's sister)
INSERT INTO characters (
    full_name, preferred_name, age, birthday,
    hair_color, eye_color,
    occupation, employer,
    character_type, is_alive, created_at, updated_at
) VALUES (
    'Neha Mehta', 'Neha', 25, '2000-01-01',
    'black', 'brown',
    'Medical Student', 'Rutgers Robert Wood Johnson Medical School',
    'minor', 1, datetime('now'), datetime('now')
);

-- Professor Sarah Chen (Priya's PhD advisor at Northwestern)
INSERT INTO characters (
    full_name, preferred_name,
    occupation, employer, job_title,
    character_type, is_alive, created_at, updated_at
) VALUES (
    'Professor Sarah Chen', 'Prof. Sarah Chen',
    'Professor', 'Northwestern University', 'Professor, Materials Science and Engineering',
    'minor', 1, datetime('now'), datetime('now')
);

-- ============================================================================
-- NEW LOCATIONS (3 records)
-- ============================================================================

-- Kate's Apartment in Evanston
INSERT INTO locations (
    name, address_street, address_city, address_state, address_zip,
    location_type, building_type, floor_count,
    ownership_type,
    neighborhood,
    nearby_landmarks,
    is_fictional, created_at, updated_at
) VALUES (
    'Kate''s Apartment', 'Hinman Avenue', 'Evanston', 'IL', '60201',
    'residence', '1-bedroom apartment, 750 sq ft, third-floor walkup in 1960s brick building', 1,
    'rented',
    'Near Central Street/Downtown Evanston',
    '["Northwestern campus (0.9 miles)", "Lake Michigan", "downtown Evanston"]',
    0, datetime('now'), datetime('now')
);

-- Linda's House in Naperville
INSERT INTO locations (
    name, address_city, address_state,
    location_type, building_type,
    ownership_type,
    neighborhood,
    is_fictional, created_at, updated_at
) VALUES (
    'Linda''s House', 'Naperville', 'IL',
    'residence', 'Family home where Kate and Mike grew up. Linda lives alone here since divorce (2001).',
    'owned',
    'Naperville suburb',
    0, datetime('now'), datetime('now')
);

-- Argonne National Laboratory
INSERT INTO locations (
    name, address_street, address_city, address_state, address_zip,
    location_type, building_type,
    is_fictional, created_at, updated_at
) VALUES (
    'Argonne National Laboratory', '9700 S Cass Ave', 'Lemont', 'IL', '60439',
    'research_facility', 'U.S. Department of Energy national laboratory. Priya works in Materials Science Division.',
    0, datetime('now'), datetime('now')
);

-- ============================================================================
-- NEW TIMELINE EVENTS (14 records)
-- Kate-Paul Relationship Milestones
-- ============================================================================

-- Note: Event ID 1 already exists for Sept 6 meeting, updating location
UPDATE timeline_events
SET event_description = 'Kate Morrison and Paul Rogala meet for the first time at Colectivo Coffee in Evanston',
    location_description = 'Colectivo Coffee, Evanston'
WHERE id = 1;

-- September 26, 2025 - First dinner date & first kiss
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-09-26', 'day', 'Kate and Paul first dinner date & first kiss',
    'First actual dinner date, 3 weeks after meeting. Casual restaurant in Evanston. Kate drove herself. First kiss in parking lot - brief, Paul read her nervousness. First physical intimacy since John.',
    'Evanston restaurant', 'relationship', 'high', 1,
    'kate-morrison-important-dates-2025.md', datetime('now')
);

-- October 17, 2025 - First overnight at Kate's apartment
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-10-17', 'day', 'First overnight at Kate''s apartment',
    'Kate invited Paul to her apartment for first time (6 weeks into relationship). Kate made dinner. First overnight stay together. Physical intimacy progressing (not full sex yet). Trust beginning to form.',
    'Kate''s apartment, Evanston', 'relationship', 'high', 1,
    'kate-morrison-important-dates-2025.md', datetime('now')
);

-- October 20, 2025 - Priya meets Paul briefly
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-10-20', 'day', 'Priya meets Paul briefly',
    'Kate met Paul at Colectivo. Priya "happened by" (Kate suspected orchestration). Brief 10-minute introduction. Priya later texted: "Ok I get it. He LOOKS at you." Priya evaluating Paul for Kate.',
    'Colectivo Coffee, Evanston', 'relationship', 'medium', 1,
    'kate-priya-paul-meeting-october-2025.md', datetime('now')
);

-- November 23, 2025 - First hockey game & first time having sex
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-11-23', 'day', 'First hockey game & first time having sex',
    'Kate''s first live hockey game. Blackhawks vs Avalanche at United Center. Kate told Paul childhood Zamboni story during intermission. Later that evening at Paul''s house: first time having sex (11 weeks into relationship). Trust fully established at physical level.',
    'United Center, then Paul''s house Skokie', 'relationship', 'high', 1,
    'kate-paul-blackhawks-game-november-23-2025.md', datetime('now')
);

-- December 6, 2025 - Kate meets Marcus
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-12-06', 'day', 'Kate meets Marcus Chen',
    'Kate met Marcus Chen (Paul''s best friend) at Paul''s house. Watching Blackhawks game together. First friend integration milestone. Marcus cautiously evaluating Kate (protective of Paul after Christie). Marcus''s assessment: "She seems nice" (cautious approval).',
    'Paul''s house, Skokie', 'relationship', 'high', 1,
    'kate-paul-marcus-blackhawks-december-6-2025.md', datetime('now')
);

-- December 9-13, 2025 - AGU Fall Meeting
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-12-10', 'day', 'Kate AGU 2025 oral presentation',
    'Kate''s AGU Fall Meeting oral presentation. 15-minute talk on "Aging Drinking Water Infrastructure as Dynamic Contaminant Reservoir." Audience ~100-150. Handled 5 questions including one tough methodology challenge. Patricia attended, praised her afterward. Most prestigious presentation yet.',
    'Washington, DC', 'professional', 'high', 1,
    'kate-morrison-conference-travel-professional-life.md', datetime('now')
);

-- December 13, 2025 - Paul picks up Kate from O'Hare
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-12-13', 'day', 'Paul picks up Kate from O''Hare',
    'Paul picked Kate up from O''Hare after AGU conference. Flight landed 8:05 PM. Kate exhausted from conference. Paul''s steady support during recovery week.',
    'O''Hare Airport', 'relationship', 'medium', 1,
    'kate-paul-ohare-pickup-december-13-2025.md', datetime('now')
);

-- December 25, 2025 - Christmas Day (Family meets Paul)
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-12-25', 'day', 'Christmas Day - Family meets Paul',
    'Family gathering at Linda''s in Naperville. Kate brought Paul to meet family (first time). Mike, Ashley, Linda all present. Family liked Paul. Kate and Paul stayed at Paul''s house that night.',
    'Linda''s house, Naperville', 'relationship', 'high', 1,
    'kate-morrison-christmas-day-2025.md', datetime('now')
);

-- December 27, 2025 - "I love you" exchange
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-12-27', 'day', 'Kate and Paul say "I love you"',
    'Afternoon: Ashley lunch, told Emma''s story and "six years or sixty years" philosophy. Evening at Paul''s house: Kate processed Ashley''s story with Paul, then said "I love you" first. Paul said it back. 16 weeks into relationship. Trust fully established at emotional level.',
    'Paul''s house, Skokie', 'relationship', 'high', 1,
    'kate-morrison-paul-evening-december-27-2025.md', datetime('now')
);

-- December 28, 2025 - Official relationship status
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-12-28', 'day', 'Kate and Paul become official boyfriend/girlfriend',
    'Following "I love you" exchange, Kate and Paul discussed and agreed they are officially boyfriend/girlfriend. Relationship status formalized.',
    'Paul''s house, Skokie', 'relationship', 'high', 1,
    'kate-morrison-sunday-morning-december-28-complete.md', datetime('now')
);

-- December 31, 2025 - New Year's Eve together
INSERT INTO timeline_events (
    event_date, event_date_precision, event_title, event_description,
    location_description, event_category, significance, is_canonical,
    source_file, created_at
) VALUES (
    '2025-12-31', 'day', 'New Year''s Eve together',
    'Kate and Paul spent New Year''s Eve at his house in Skokie. Quiet celebration, cooked dinner together, watched ball drop. Said "I love you" at midnight. Kate''s apartment building has water damage issue ongoing.',
    'Paul''s house, Skokie', 'relationship', 'medium', 1,
    'kate-morrison-paul-new-years-eve-december-31-2025.md', datetime('now')
);

-- ============================================================================
-- NEW RELATIONSHIPS (10 records)
-- ============================================================================

-- Kate - Linda (mother-daughter)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, current_status, notes
) VALUES (
    1, (SELECT id FROM characters WHERE preferred_name = 'Linda'),
    'family', 'mother_daughter',
    '1993-03-15', 'active',
    'Complex relationship. Linda loving but anxious and overwhelming. Kate loves her but finds her exhausting. Weekly Sunday phone calls maintained.'
);

-- Kate - Mike (siblings)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    current_status, notes
) VALUES (
    1, 9,
    'family', 'siblings',
    'active',
    'Siblings with opposite personalities. Mike is sociable and easy, Kate is analytical and guarded. They love each other but don''t fully understand each other.'
);

-- Kate - Priya (best friends)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, current_status, notes
) VALUES (
    1, (SELECT id FROM characters WHERE preferred_name = 'Priya'),
    'friendship', 'best_friends',
    '2022-09-01', 'active',
    'Kate''s only close friend. Met in PhD program at Northwestern. Priya is warm and extroverted, balances Kate''s introversion. Priya understands Kate''s anxiety without judgment.'
);

-- Mike - Ashley (engaged)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, current_status, notes
) VALUES (
    9, 6,
    'romantic', 'engaged',
    '2024-12-01', 'active',
    'Engaged December 2024. Wedding planned June 2026. Live together in West Loop. Strong, supportive relationship.'
);

-- Priya - Raj (engaged)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, current_status, notes
) VALUES (
    (SELECT id FROM characters WHERE preferred_name = 'Priya'),
    (SELECT id FROM characters WHERE preferred_name = 'Raj'),
    'romantic', 'engaged',
    '2024-12-15', 'active',
    'Engaged December 15, 2024. Wedding planned June 2026. Live together in Evanston. Met through mutual friends in grad school.'
);

-- Priya - Neha (siblings)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    current_status, notes
) VALUES (
    (SELECT id FROM characters WHERE preferred_name = 'Priya'),
    (SELECT id FROM characters WHERE preferred_name = 'Neha'),
    'family', 'siblings',
    'active',
    'Sisters. Priya is older by 3 years. Close relationship.'
);

-- Paul - Marcus (best friends)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, current_status, notes
) VALUES (
    2, 7,
    'friendship', 'best_friends',
    '2013-11-01', 'active',
    'Paul''s closest male friend. Met in consulting in 2013. Marcus lived with Paul during his crisis in 2019. Monthly Blackhawks games tradition. Marcus credits Paul with saving his life.'
);

-- Priya - Prof. Sarah Chen (advisor)
INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, end_date, current_status, notes
) VALUES (
    (SELECT id FROM characters WHERE preferred_name = 'Priya'),
    (SELECT id FROM characters WHERE full_name = 'Professor Sarah Chen'),
    'professional', 'advisor_advisee',
    '2021-09-01', '2025-09-21', 'completed',
    'Priya''s PhD advisor at Northwestern. Composite materials expert. Priya defended September 2025.'
);

-- Update Kate-Paul relationship to reflect current status (boyfriend/girlfriend since Dec 28)
UPDATE relationships
SET current_status = 'boyfriend_girlfriend',
    notes = 'Kate and Paul became official boyfriend/girlfriend December 28, 2025. First met September 6, 2025. First kiss September 26. First "I love you" December 27, 2025.'
WHERE character_a_id = 1 AND character_b_id = 2;

-- ============================================================================
-- NEW SCHEDULES (3 records)
-- ============================================================================

-- Kate's morning running routine
INSERT INTO schedules (
    character_id, schedule_type, schedule_name, days_of_week,
    start_time, end_time, location_description,
    description, exceptions, is_current
) VALUES (
    1, 'exercise', 'Morning Run',
    '["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]',
    '06:00', '07:00',
    'Lakefront Trail or neighborhood streets',
    'Kate runs 4-6 miles most mornings. Outdoor running strongly preferred. Treadmill only if below 15°F. Running is emotional regulation and processing time.',
    '["severe weather", "illness", "travel"]',
    1
);

-- Kate's coffee ritual
INSERT INTO schedules (
    character_id, schedule_type, schedule_name, days_of_week,
    start_time, end_time, location_description,
    description, is_current
) VALUES (
    1, 'daily_routine', 'Morning Coffee Ritual',
    '["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]',
    '06:45', '07:15',
    'Home (or Paul''s house)',
    'Sacred morning ritual. Chemex pour-over, precisely measured, specific process. Only time Kate allows herself precision and attention to a non-work activity.',
    1
);

-- Kate's Sunday call with Linda
INSERT INTO schedules (
    character_id, schedule_type, schedule_name, days_of_week,
    start_time, end_time, location_description,
    description, exceptions, is_current
) VALUES (
    1, 'weekly_commitment', 'Sunday call with Mom',
    '["Sunday"]',
    '14:00', '14:30',
    'Phone',
    'Weekly phone call with Linda. Kate maintains this obligation despite finding calls draining. Linda needs connection more than Kate does. Calls average 20-30 minutes.',
    '["travel", "major events"]',
    1
);

-- ============================================================================
-- NEW CHARACTER NEGATIVES (2 records)
-- ============================================================================

-- Kate doesn't use dating apps
INSERT INTO character_negatives (
    character_id, negative_category, negative_behavior, strength, explanation
) VALUES (
    1, 'social', 'Does NOT use dating apps',
    'strong',
    'Kate finds dating apps overwhelming and anxiety-inducing. Met Paul organically at coffee shop. Prefers natural connections despite limiting opportunities.'
);

-- Kate doesn't host gatherings
INSERT INTO character_negatives (
    character_id, negative_category, negative_behavior, strength, explanation
) VALUES (
    1, 'social', 'Does NOT host social gatherings at her apartment',
    'strong',
    'Kate''s apartment is her retreat, not a social space. Has never hosted a party or dinner party. Priya is only friend who has spent significant time there.'
);

COMMIT;

-- Re-enable foreign key checks
PRAGMA foreign_keys = ON;

-- ============================================================================
-- VERIFICATION QUERIES (run these to confirm changes)
-- ============================================================================

SELECT '=== NEW CHARACTERS ===';
SELECT id, full_name, preferred_name, character_type FROM characters WHERE id > 43;

SELECT '=== NEW LOCATIONS ===';
SELECT id, name, address_city FROM locations WHERE id > 12;

SELECT '=== NEW TIMELINE EVENTS ===';
SELECT id, event_date, event_title FROM timeline_events WHERE id > 12 ORDER BY event_date;

SELECT '=== KATE AGE UPDATE ===';
SELECT full_name, age FROM characters WHERE id = 1;

SELECT '=== KATE-PAUL RELATIONSHIP STATUS ===';
SELECT r.id, c1.preferred_name as char1, c2.preferred_name as char2, r.current_status, r.notes
FROM relationships r
JOIN characters c1 ON r.character_a_id = c1.id
JOIN characters c2 ON r.character_b_id = c2.id
WHERE r.character_a_id = 1 AND r.character_b_id = 2;

-- ============================================================================
-- END OF UPDATE SCRIPT
-- ============================================================================
