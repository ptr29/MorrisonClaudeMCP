-- =====================================================
-- Kate Morrison Canonical Facts Database - ADDITIONS
-- Second Pass: Additional Entries Found
-- Generated: 2026-01-15
-- CORRECTED TO MATCH ACTUAL SCHEMA
-- =====================================================

-- =====================================================
-- CHARACTERS TABLE - ADDITIONS
-- Note: No 'notes' column exists in characters table
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age, occupation, character_type
) VALUES
-- Main Characters (missing from base data)
('Emma Hartwell', 'Emma', '1997-01-01', 28, 'Former teacher, deceased 2020', 'secondary'),
('Patrick O''Brien', 'Patrick', '1965-01-01', 60, 'Chicago Fire Department Captain (retired)', 'secondary'),
('Maureen Murphy O''Brien', 'Maureen', '1967-01-01', 58, 'Elementary school teacher (Oak Park)', 'secondary'),
('Ashley O''Brien', 'Ashley', '1997-11-15', 28, 'Account Executive at marketing agency', 'secondary'),
('Marcus Chen', 'Marcus', '1989-06-20', 36, 'Investment Banker', 'secondary'),
('Alice Thomas', 'Alice', '1994-03-08', 31, 'Account Director at DDB Chicago', 'secondary'),
('Mike Morrison', 'Mike', '1996-07-12', 29, 'Marketing Manager at TechFlow Solutions', 'secondary'),

-- Hart well Family
('Charles Harrison Hartwell III', 'Charles', '1963-01-01', 62, 'Managing Partner, corporate litigation law firm', 'supporting'),
('Victoria Lancaster Hartwell', 'Victoria', '1966-01-01', 59, 'Philanthropist, charity board member', 'supporting'),
('Caroline Victoria Hartwell-Montgomery', 'Caroline', '1994-01-01', 31, 'Homemaker, Junior League, charity boards', 'supporting'),
('Stratton Montgomery III', 'Stratton', '1992-01-01', 33, 'Investment banker', 'supporting'),

-- O'Brien Brothers
('Daniel Francis O''Brien', 'Danny', '1962-01-01', 63, 'Retired CPD Sergeant', 'supporting'),
('Joan Murphy O''Brien', 'Joan', '1963-01-01', 62, 'Retired elementary school principal', 'supporting'),
('Sean Michael O''Brien', 'Sean', '1969-01-01', 56, 'Union carpenter, Local 1 Chicago', 'supporting'),
('Maria Gonzalez O''Brien', 'Maria', '1971-01-01', 54, 'Dental hygienist', 'supporting'),

-- Danny's Children
('Meghan O''Brien Callahan', 'Meghan', '1987-01-01', 38, 'High school English teacher', 'minor'),
('Daniel O''Brien Jr.', 'DJ', '1990-01-01', 35, 'Chicago firefighter', 'minor'),
('Kevin O''Brien', 'Kevin', '1993-01-01', 32, 'Accountant', 'minor'),
('Erin O''Brien', 'Erin', '1996-01-01', 29, 'Nurse at Northwestern Memorial', 'minor'),

-- Sean's Children
('Patrick O''Brien Jr.', 'Patrick Jr.', '1994-01-01', 31, 'Electrician', 'minor'),
('Grace O''Brien Martinez', 'Grace', '1997-01-01', 28, 'Social worker at DCFS', 'minor'),
('Michael O''Brien', 'Mikey', '2000-01-01', 25, 'Medical student, Loyola Stritch (3rd year)', 'minor'),
('Ana O''Brien', 'Ana', '2003-01-01', 22, 'Marketing (entry-level)', 'minor'),

-- Maureen's Side
('Marie Catherine Murphy Brennan', 'Marie', '1970-01-01', 55, 'Nurse Practitioner at Oak Park clinic', 'minor'),
('James Brennan', 'James', '1968-01-01', 57, 'High school history teacher at Oak Park-River Forest HS', 'minor'),
('Sarah Brennan', 'Sarah', '1996-01-01', 29, 'Elementary school teacher', 'minor'),
('Thomas Brennan', 'Tom', '1998-01-01', 27, 'Software engineer', 'minor'),
('Katie Brennan', 'Katie', '2001-01-01', 24, 'Social worker (starting career)', 'minor'),

-- Kate's Committee
('Dr. Patricia Chen', 'Patricia', '1979-01-01', 46, 'Associate Professor, Civil & Environmental Engineering, Northwestern', 'secondary'),
('Dr. Michael Brennan', 'Michael Brennan', '1967-01-01', 58, 'Professor, Civil & Environmental Engineering, Northwestern', 'minor'),
('Dr. Sarah Kline', 'Sarah Kline', '1988-01-01', 37, 'Associate Professor, Chemistry, Northwestern', 'minor'),
('Dr. James Rodriguez', 'James Rodriguez', '1984-01-01', 41, 'Associate Professor, Civil & Environmental Engineering, Northwestern', 'minor'),
('Dr. Lisa Wu', 'Lisa Wu', '1970-01-01', 55, 'Senior Research Scientist, Illinois State Water Survey', 'minor'),
('Dr. David Chen', 'David Chen', '1993-01-01', 32, 'Postdoctoral Researcher, Chen Lab, Northwestern', 'minor'),
('Jessica Okonkwo', 'Jessica', '1997-01-01', 28, 'PhD student (Year 6), Chen Lab, Northwestern', 'minor'),
('Rashid Ahmed', 'Rashid', '2000-01-01', 25, 'PhD student (Year 1), Chen Lab, Northwestern', 'minor'),

-- Other Characters
('Sarah Goldstein', 'Sarah Goldstein', '1987-01-01', 38, 'Marketing executive', 'minor'),
('Steve Thomas', 'Steve', '1991-01-01', 34, 'Business professional (client-facing, travels extensively)', 'secondary'),
('Ms. Rodriguez', 'Ms. Rodriguez', NULL, NULL, 'High school teacher (Kate''s time)', 'minor'),
('Ahmad', 'Ahmad', NULL, NULL, 'PhD student (Year 5), Environmental Engineering, Northwestern', 'minor'),
('Diane', 'Diane', NULL, NULL, 'Administrative Coordinator, Environmental Engineering Dept, Northwestern', 'minor');

-- =====================================================
-- LOCATIONS TABLE - ADDITIONS
-- Note: No 'ownership_status' or 'notes' columns
-- Use 'ownership_type' instead
-- =====================================================

INSERT INTO locations (
    name, address_street, address_city, address_state, location_type
) VALUES
('Hartwell Estate', 'Sheridan Road', 'Kenilworth', 'IL', 'residential_single_family'),
('Caroline Hartwell-Montgomery House', NULL, 'Glencoe', 'IL', 'residential_single_family'),
('Alice & Steve Thomas House', NULL, 'Wilmette', 'IL', 'residential_single_family'),
('DDB Chicago Office', NULL, 'Chicago', 'IL', 'commercial_office'),
('Danny O''Brien House', NULL, 'Chicago', 'IL', 'residential_single_family'),
('Sean O''Brien House', NULL, 'Chicago', 'IL', 'residential_single_family'),
('Marie Brennan House', NULL, 'Oak Park', 'IL', 'residential_single_family'),
('Loyola University Chicago', '1032 W Sheridan Rd', 'Chicago', 'IL', 'educational_university'),
('New Trier High School', '385 Winnetka Ave', 'Winnetka', 'IL', 'educational_high_school'),
('Niles North High School', '9800 Lawler Ave', 'Skokie', 'IL', 'educational_high_school'),
('Illinois State Water Survey', '2204 Griffith Dr', 'Champaign', 'IL', 'research_facility');

-- =====================================================
-- RELATIONSHIPS TABLE - ADDITIONS
-- Note: Column is 'current_status' not 'status'
-- =====================================================

INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, end_date, current_status, notes
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Emma Hartwell'),
    (SELECT id FROM characters WHERE full_name = 'Charles Harrison Hartwell III'),
    'family', 'father_daughter', NULL, NULL, 'active',
    'Dysfunctional relationship. Charles dismissive, controlling through money.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Emma Hartwell'),
    (SELECT id FROM characters WHERE full_name = 'Victoria Lancaster Hartwell'),
    'family', 'mother_daughter', NULL, NULL, 'active',
    'Dysfunctional. Victoria confides inappropriately, uses Emma as emotional support.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Emma Hartwell'),
    (SELECT id FROM characters WHERE full_name = 'Caroline Victoria Hartwell-Montgomery'),
    'family', 'siblings', NULL, NULL, 'active',
    'Competitive but occasionally allied against parents.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Patrick O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Daniel Francis O''Brien'),
    'family', 'siblings', NULL, NULL, 'active',
    'Two of three O''Brien brothers. Constant banter, deep loyalty.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Patrick O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Sean Michael O''Brien'),
    'family', 'siblings', NULL, NULL, 'active',
    'Patrick is middle brother, Sean is youngest.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    (SELECT id FROM characters WHERE full_name = 'Steve Thomas'),
    'romantic_partner', 'married', '2018-06-01', NULL, 'active',
    'Strong partnership built on mutual respect and shared goals.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Patricia Chen'),
    'professional', 'advisor_advisee', '2022-09-06', NULL, 'active',
    'Primary dissertation advisor. Demanding but supportive.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Michael Brennan'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'Committee member. Practical questions about implementation.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Sarah Kline'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'Committee member. Chemistry expertise.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. James Rodriguez'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'Committee member. Statistics expertise.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Lisa Wu'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'External committee member.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. David Chen'),
    'professional', 'lab_colleague', '2022-09-01', NULL, 'active',
    'Postdoc who trained Kate on equipment.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Marcus Chen'),
    (SELECT id FROM characters WHERE full_name = 'Sarah Goldstein'),
    'ex_partner', 'ex_spouse', '2016-10-01', '2019-01-01', 'ended',
    'Met at wedding summer 2015. Divorced 2019.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Ashley O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Sarah Brennan'),
    'family', 'cousins', NULL, NULL, 'active',
    'Closest cousin to Ashley - best friend level.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Ashley O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Erin O''Brien'),
    'family', 'cousins', NULL, NULL, 'active',
    'Close cousin, similar age and personality.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Ashley O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Grace O''Brien Martinez'),
    'family', 'cousins', NULL, NULL, 'active',
    'Close cousin, same age, similar values.';

-- =====================================================
-- TIMELINE_EVENTS TABLE - ADDITIONS
-- Note: Column is 'significance' not 'importance'
-- Has created_at but NO updated_at
-- =====================================================

INSERT INTO timeline_events (
    event_date, event_title, event_description, significance
) VALUES
('2024-04-21', 'Kate comprehensive exam', 'Kate passed comprehensive exam. Three-hour oral defense. Committee of five grilled her. Patricia toughest questioner. Major milestone - now ABD.', 'high'),
('2025-10-15', 'Kate proposal defense', 'Kate defended detailed research plan for remaining PhD years. Committee approved with minor revisions.', 'high'),
('2023-12-13', 'Kate AGU 2023 poster', 'Kate''s first major conference. Poster presentation on microplastics and PFAS in Chicago drinking water. San Francisco, Moscone Center. Overwhelmed by scale (20,000+ attendees). Skipped AGU student mixer.', 'medium'),
('2024-04-09', 'Kate Illinois Water Conference', 'Kate presented at regional conference at UIUC (her undergrad). Connected with Dr. Lisa Wu who later joined committee. Strange being back at undergrad campus.', 'medium'),
('2024-11-18', 'Kate SETAC oral presentation', 'Kate''s first major oral at national conference. Portland, OR. Very nervous but presented clearly. Tough Q&A, handled well. Skipped conference banquet and EPA mixer.', 'medium'),
('2015-10-01', 'Ashley O''Brien family dinner', 'Emma met O''Brien family. Patrick: "So you''re the friend Ashley won''t shut up about." Emma unused to warmth, volume, casual affection.', 'low'),
('2016-03-15', 'Emma O''Brien spring break', 'Emma joined O''Brien family vacation to Gulf Shores. First family vacation she actually enjoyed. Cried on last night.', 'low'),
('2016-01-15', 'Alice and Steve meet', 'Alice met Steve at bar near UIUC campus. He was Northwestern MBA student visiting friends. Started dating casually.', 'medium'),
('2017-12-15', 'Alice and Steve engaged', 'Romantic proposal in Chicago with Christmas lights.', 'medium'),
('2018-06-15', 'Alice and Steve wedding', 'Elegant wedding, 150 people. Beautiful day.', 'high');

-- Alice moving to Wilmette - WITH location_id lookup
INSERT INTO timeline_events (
    event_date, event_title, event_description, location_id, significance
)
SELECT
    '2020-10-15', 'Alice and Steve move to Wilmette',
    'Moved from Chicago to Wilmette. Needed more space with two kids. Better schools.',
    (SELECT id FROM locations WHERE name = 'Alice & Steve Thomas House'),
    'medium';

-- =====================================================
-- POSSESSIONS TABLE - ADDITIONS
-- Note: Columns are item_description, storage_location, significance_notes
-- NO created_at or updated_at
-- =====================================================

INSERT INTO possessions (
    owner_id, item_name, item_description, storage_location, sentimental_value, significance_notes
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Paul Rogala'),
    'Turing',
    '2014 Mini Cooper S',
    'Paul''s house', 'high',
    'Paul''s beloved Mini Cooper S named Turing.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Paul Rogala'),
    'Volvo',
    '2021 Mini Cooper S - Blue (black roof)',
    'Paul''s house', 'medium',
    'Paul''s second car, also a Mini Cooper S.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Hario gooseneck kettle',
    'Paired with Chemex for pour-over coffee ritual',
    'Paul''s house (as of Jan 2026)', 'medium',
    'Part of sacred morning coffee ritual. Precise water temperature.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Baratza Encore grinder',
    'Burr coffee grinder for consistent grind',
    'Paul''s house (as of Jan 2026)', 'medium',
    'Grinds beans fresh each morning. Exact amount every time.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Running medals collection',
    '9 race medals including one marathon. Proof of finishing things.',
    'Kate''s apartment bedroom wall', 'high',
    'Only things on walls besides watershed map. Naperville Half (2014), Chicago Marathon (2016 - 3:54:32, cried at finish), various 10Ks and halfs.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Mike''s UIUC sweatshirt',
    'Navy blue, Fighting Illini, worn soft. Mike gave for 18th birthday. Small bleach stain, missing drawstring.',
    'Kate''s apartment bottom drawer', 'high',
    'Wore constantly freshman year. Evidence Mike once paid attention. Can''t throw away.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Nancy Drew collection',
    '28 books, yellowed pages. Ages 8-12. First fictional character Kate identified with.',
    'Mom''s house Naperville (closet)', 'medium',
    'Some volumes falling apart. Can''t throw away despite not rereading.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Harry Potter series',
    'All 7, original US editions. HP1 extremely worn (read 15+ times). Marginalia in pencil.',
    'Mom''s house Naperville (closet)', 'medium',
    'Connection to period when she read for joy, not escape.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Silent Spring book',
    'Rachel Carson. High school Environmental Science class (2009). Changed her life direction. Yellow highlighter throughout. Notes from 17-year-old Kate.',
    'Mom''s house Naperville (closet)', 'high',
    'Reminder of why she chose environmental engineering path.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Science fair trophies',
    'Three trophies from middle school. 2nd place (6th), 1st place (7th & 8th). Water quality focused.',
    'Mom''s house Naperville (childhood bedroom)', 'medium',
    'Evidence 13-year-old Kate cared about water quality. Tarnished gold plastic. Too childish to display as adult, too meaningful to discard.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'MacBook Pro 2021',
    'Primary work laptop for dissertation writing',
    'Kate''s apartment desk', 'low',
    'With external monitor, wireless keyboard and mouse.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'Great Lakes watershed map',
    'Framed. Only wall art in apartment besides running medals.',
    'Kate''s apartment living room wall', 'medium',
    'Evidence of professional identity in personal space.';

-- =====================================================
-- SCHEDULES TABLE - ADDITIONS
-- Note: Use location_description for text, description for notes
-- Column is 'is_current' not 'is_mandatory'
-- NO created_at or updated_at
-- =====================================================

INSERT INTO schedules (
    character_id, schedule_type, schedule_name, days_of_week, start_time, end_time,
    location_description, is_current, description
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'weekly_commitment', 'Lab meeting', '["thursday"]', '14:00', '16:00',
    'Tech Building, Northwestern', 1,
    'Data presentations, troubleshooting, paper planning. Kate prepares carefully. Presents every 3-4 weeks.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'weekly_commitment', 'Thursday coffee social', '["thursday"]', '15:00', '16:00',
    'Chen Lab kitchen', 1,
    'Informal gathering. Patricia brings good coffee. Kate attends ~80% of time, stays 30-40 minutes, doesn''t stay for extended socializing.';

-- =====================================================
-- CHARACTER_NEGATIVES TABLE - ADDITIONS
-- Note: Columns are negative_category, explanation, exception_conditions
-- NO created_at or updated_at
-- =====================================================

INSERT INTO character_negatives (
    character_id, negative_category, negative_behavior, strength, exception_conditions, explanation
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'social',
    'Does NOT attend student mixers at conferences',
    'strong', NULL,
    'Skipped AGU student mixer (too anxiety-inducing). Too overwhelming, too many people.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'social',
    'Does NOT stay at networking events longer than 45-60 minutes',
    'strong',
    'If required for specific professional reason',
    'Maximum capacity. Leaves after reasonable effort. Did minimum necessary, not maximum beneficial.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'social',
    'Does NOT attend conference banquets or social dinners',
    'strong',
    'Required for committee or advisor',
    'Skipped SETAC conference banquet, EPA mixer. Patricia frustrated but Kate "just can''t."'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katherine Marie Morrison'),
    'social',
    'Does NOT pursue connections made at conferences',
    'preference',
    'If professional collaboration clearly valuable',
    'Exchanged cards with 8-10 people, followed up with 4-5 briefly. Pattern of letting connections drift.';

-- =====================================================
-- EDUCATION TABLE - ADDITIONS
-- Note: Has advisor and notes columns
-- NO created_at or updated_at
-- =====================================================

INSERT INTO education (
    character_id, institution, degree_type, field_of_study, start_year, end_year, honors, notes
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    'University of Illinois Urbana-Champaign', 'B.A.', 'Advertising',
    2012, 2016, 'Dean''s List, cum laude',
    'Minor in Psychology. Ad Club vice president senior year. Study abroad London Spring 2015. Met Steve senior year.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    'Google Ads', NULL, 'Google Ads certification',
    NULL, NULL, NULL,
    'Industry certification for advertising work.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    'Facebook Blueprint', NULL, 'Facebook Blueprint certification',
    NULL, NULL, NULL,
    'Industry certification for advertising work.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Steve Thomas'),
    'Northwestern University', 'MBA', 'Business',
    2015, 2017, 'Kellogg School',
    'Met Alice while visiting UIUC friends January 2016.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Erin O''Brien'),
    'Unknown', 'BSN', 'Nursing',
    NULL, NULL, NULL,
    'Works at Northwestern Memorial Hospital.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Michael O''Brien'),
    'Loyola University Chicago', 'M.D.', 'Medicine',
    2023, 2027, 'Stritch School of Medicine',
    'Year 3 medical student. Sean and Maria''s son.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katie Brennan'),
    'University of Chicago', 'MSW', 'Social Work',
    2022, 2024, NULL,
    'Marie and James''s daughter. Starting social work career.'
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Thomas Brennan'),
    'Unknown', 'B.S.', 'Computer Science',
    NULL, NULL, NULL,
    'Software engineer. Marie and James''s son. Engaged to Jennifer.';

-- =====================================================
-- END OF CORRECTED SEED DATA
-- =====================================================
