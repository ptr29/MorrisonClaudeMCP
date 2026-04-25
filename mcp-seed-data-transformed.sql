-- =====================================================
-- Kate Morrison Canonical Facts Database - ADDITIONS
-- Second Pass: Additional Entries Found
-- Generated: 2026-01-15
-- TRANSFORMED TO MATCH CURRENT SCHEMA
-- =====================================================

-- =====================================================
-- CHARACTERS TABLE - ADDITIONS
-- =====================================================

-- =====================================================
-- HARTWELL FAMILY (Emma's family)
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age,
    occupation, character_type, notes,
    created_at, updated_at
) VALUES
-- Charles Hartwell III (Emma's father)
(
    'Charles Harrison Hartwell III', 'Charles', '1963-01-01', 62,
    'Managing Partner, corporate litigation law firm',
    'supporting',
    datetime('now'), datetime('now')
),
-- Victoria Hartwell (Emma's mother)
(
    'Victoria Lancaster Hartwell', 'Victoria', '1966-01-01', 59,
    'Philanthropist, charity board member',
    'supporting',
    'Emma''s mother. North Shore old money family, debutante. Passive-aggressive perfectionist. Self-medicates with Xanax and wine. Enables Charles''s behavior. Confides in Emma inappropriately.',
    datetime('now'), datetime('now')
),
-- Caroline Hartwell-Montgomery (Emma's sister)
(
    'Caroline Victoria Hartwell-Montgomery', 'Caroline', '1994-01-01', 31,
    'Homemaker, Junior League, charity boards',
    'supporting',
    'Emma''s older sister (3 years older). Married Stratton Montgomery III. Two sons Harrison (7) and Charles (5). $2.5M home. Followed the script perfectly but actually unhappy. Day drinking normalized.',
    datetime('now'), datetime('now')
),
-- Stratton Montgomery III
(
    'Stratton Montgomery III', 'Stratton', '1992-01-01', 33,
    'Investment banker',
    'supporting',
    'Caroline''s husband. Works 80-hour weeks. Father to Harrison and Charles. Old money background.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Patrick's Brothers
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age,
    occupation, character_type, notes,
    created_at, updated_at
) VALUES
-- Danny O'Brien (Patrick's older brother)
(
    'Daniel Francis O''Brien', 'Danny', '1962-01-01', 63,
    'Retired CPD Sergeant',
    'supporting',
    'Patrick''s older brother. CPD career 1982-2020 (38 years). Classic cop - gruff, opinionated, loves arguing politics. White Sox fanatic, season tickets. Drinks whiskey, tells long police stories. Knights of Columbus member. Calls Ashley "Red".',
    datetime('now'), datetime('now')
),
-- Joan O'Brien (Danny's wife)
(
    'Joan Murphy O''Brien', 'Joan', '1963-01-01', 62,
    'Retired elementary school principal',
    'supporting',
    'Danny''s wife since 1985. The calm to Danny''s storm. Excellent cook - Irish soda bread, shepherd''s pie. Close with Maureen. Book club member, garden enthusiast. Manages Danny''s retirement schedule.',
    datetime('now'), datetime('now')
),
-- Sean O'Brien (Patrick's younger brother)
(
    'Sean Michael O''Brien', 'Sean', '1969-01-01', 56,
    'Union carpenter, Local 1 Chicago',
    'supporting',
    'Patrick''s younger brother. Union carpenter 35+ years. Built his own house. Most laid-back of three brothers. Plays guitar at family gatherings. Family peacemaker. Calls Ashley "Ash-tray". Making wooden gift for her wedding.',
    datetime('now'), datetime('now')
),
-- Maria O'Brien (Sean's wife)
(
    'Maria Gonzalez O''Brien', 'Maria', '1971-01-01', 54,
    'Dental hygienist',
    'supporting',
    'Sean''s wife since 1992. Mexican-American. Brings cultural diversity to Irish clan. Makes tamales for Christmas Eve (now family tradition). Bilingual. Gets along with everyone.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Danny's Children (Ashley's Cousins)
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age,
    occupation, character_type, notes,
    created_at, updated_at
) VALUES
(
    'Meghan O''Brien Callahan', 'Meghan', '1987-01-01', 38,
    'High school English teacher',
    'minor',
    'Danny''s oldest daughter. Married to Tom Callahan (40). Three children: Danny Jr. (15), Bridget (13), Finn (10). Hosting Thanksgiving 2025. Ashley admires her, age gap means not super close.',
    datetime('now'), datetime('now')
),
(
    'Daniel O''Brien Jr.', 'DJ', '1990-01-01', 35,
    'Chicago firefighter',
    'minor',
    'Danny''s son. Named after father, goes by DJ. Carries on family firefighter tradition. Married to Rachel (33), two kids Connor (8), Riley (5). Danny Sr. and Patrick very proud.',
    datetime('now'), datetime('now')
),
(
    'Kevin O''Brien', 'Kevin', '1993-01-01', 32,
    'Accountant',
    'minor',
    'Danny''s son. Accountant at mid-size firm. Married to Lauren (31), daughter Nora (3). Quieter than other O''Briens. Does taxes for family members.',
    datetime('now'), datetime('now')
),
(
    'Erin O''Brien', 'Erin', '1996-01-01', 29,
    'Nurse at Northwestern Memorial',
    'minor',
    'Danny''s youngest. Single, dating on and off. Fun, social, closest to Ashley in personality. Bridesmaid in Ashley''s wedding. They text regularly, share dating stories, meet for drinks/brunch.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Sean's Children
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age,
    occupation, character_type, notes,
    created_at, updated_at
) VALUES
(
    'Patrick O''Brien Jr.', 'Patrick Jr.', '1994-01-01', 31,
    'Electrician',
    'minor',
    'Sean''s oldest. Named after Uncle Patrick. Works with cousin Brendan sometimes. Married to Amy (30), two kids Sean Jr. (6), Ella (3). Solid guy, family-oriented, quiet.',
    datetime('now'), datetime('now')
),
(
    'Grace O''Brien Martinez', 'Grace', '1997-01-01', 28,
    'Social worker at DCFS',
    'minor',
    'Sean''s daughter. Married to Carlos Martinez (29). Passionate about social justice. Close with Ashley - same age, similar values. Bridesmaid in wedding. Balances Mexican and Irish heritage comfortably.',
    datetime('now'), datetime('now')
),
(
    'Michael O''Brien', 'Mikey', '2000-01-01', 25,
    'Medical student, Loyola Stritch (3rd year)',
    'minor',
    'Sean''s son. Goes by "Mikey" to distinguish from other Michaels. Smart, ambitious. Family very proud. Single, focused on school.',
    datetime('now'), datetime('now')
),
(
    'Ana O''Brien', 'Ana', '2003-01-01', 22,
    'Marketing (entry-level)',
    'minor',
    'Sean''s youngest. Recent DePaul grad (2024). Lives with roommates. Youngest cousin on O''Brien side. Finding her way in adult life. Ashley offers career advice when asked.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Maureen's Side (Murphy/Brennan)
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age,
    occupation, character_type, notes,
    created_at, updated_at
) VALUES
(
    'Marie Catherine Murphy Brennan', 'Marie', '1970-01-01', 55,
    'Nurse Practitioner at Oak Park clinic',
    'minor',
    'Maureen''s younger sister (only sibling). Oak Park native, never left. Close with Maureen - talk daily, see each other constantly. More reserved than Maureen but still warm. Thoughtful, reads constantly. Lived two blocks from Ashley growing up.',
    datetime('now'), datetime('now')
),
(
    'James Brennan', 'James', '1968-01-01', 57,
    'High school history teacher at Oak Park-River Forest HS',
    'minor',
    'Marie''s husband since 1993. Intellectual, reads history books, watches documentaries. Coaches basketball. Gets along with O''Brien brothers despite different temperament.',
    datetime('now'), datetime('now')
),
(
    'Sarah Brennan', 'Sarah', '1996-01-01', 29,
    'Elementary school teacher',
    'minor',
    'Marie''s oldest daughter. Single, dating casually. Closest cousin to Ashley - best friend level. Grew up two blocks apart (practically neighbors). Bridesmaid. Talk/text daily, know everything about each other.',
    datetime('now'), datetime('now')
),
(
    'Thomas Brennan', 'Tom', '1998-01-01', 27,
    'Software engineer',
    'minor',
    'Marie''s son. Engaged to Jennifer (26), wedding Fall 2026. Quieter than other cousins, more introverted.',
    datetime('now'), datetime('now')
),
(
    'Katie Brennan', 'Katie', '2001-01-01', 24,
    'Social worker (starting career)',
    'minor',
    'Marie''s youngest. Recent MSW from UChicago. Lives at home temporarily while job searching. Sweet, idealistic, passionate about helping people.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- NORTHWESTERN ACADEMIC - Kate's Committee and Lab
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age,
    occupation, character_type, notes,
    created_at, updated_at
) VALUES
(
    'Dr. Patricia Chen', 'Patricia', '1979-01-01', 46,
    'Associate Professor, Civil & Environmental Engineering, Northwestern',
    'secondary',
    'Kate''s dissertation advisor. Recruited to Northwestern for Great Lakes research access. Research: microplastics and PFAS in drinking water. Management style: demanding but supportive, high standards, direct communication. Committee chair.',
    datetime('now'), datetime('now')
),
(
    'Dr. Michael Brennan', 'Michael Brennan', '1967-01-01', 58,
    'Professor, Civil & Environmental Engineering, Northwestern',
    'minor',
    'Kate''s committee member #2. Expertise: drinking water treatment, infrastructure, regulatory compliance. Practical, industry-focused, asks operational questions. Kate took his Water Treatment Engineering class Year 1.',
    datetime('now'), datetime('now')
),
(
    'Dr. Sarah Kline', 'Sarah Kline', '1988-01-01', 37,
    'Associate Professor, Chemistry, Northwestern',
    'minor',
    'Kate''s committee member #3. Expertise: analytical chemistry, environmental contaminants, mass spectrometry. Detail-oriented, methodologically rigorous. Toughest questioner about data quality. Kate audited her Advanced Analytical Chemistry seminar.',
    datetime('now'), datetime('now')
),
(
    'Dr. James Rodriguez', 'James Rodriguez', '1984-01-01', 41,
    'Associate Professor, Civil & Environmental Engineering, Northwestern',
    'minor',
    'Kate''s committee member #4. Expertise: contaminant transport modeling, environmental fate, statistics. Theoretical, big-picture thinker. Most comfortable committee member after Patricia. Kate took his Contaminant Fate & Transport class Year 1.',
    datetime('now'), datetime('now')
),
(
    'Dr. Lisa Wu', 'Lisa Wu', '1970-01-01', 55,
    'Senior Research Scientist, Illinois State Water Survey',
    'minor',
    'Kate''s external committee member #5. Expertise: Great Lakes water quality, applied research, policy interface. Collaborative, supportive of Kate''s work. Suggested Kate consider academic job market.',
    datetime('now'), datetime('now')
),
(
    'Dr. David Chen', 'David Chen', '1993-01-01', 32,
    'Postdoctoral Researcher, Chen Lab, Northwestern',
    'minor',
    'Postdoc in Patricia Chen''s lab (no relation to Patricia). MIT PhD. Research: PFAS transport mechanisms. Trained Kate on analytical equipment (GC-MS/MS, Py-GC-MS). Patient instructor. Go-to for method troubleshooting. Collaborative on papers. Professional relationship only.',
    datetime('now'), datetime('now')
),
(
    'Jessica Okonkwo', 'Jessica', '1997-01-01', 28,
    'PhD student (Year 6), Chen Lab, Northwestern',
    'minor',
    'Fellow PhD student. Year 6, defending Spring 2026. Rice undergrad. Research: nanomaterial fate in aquatic systems. Warm, extroverted, social organizer. Tried to befriend Kate, eventually accepted cordial but distant dynamic. Kate couldn''t provide friend Kate needed.',
    datetime('now'), datetime('now')
),
(
    'Rashid Ahmed', 'Rashid', '2000-01-01', 25,
    'PhD student (Year 1), Chen Lab, Northwestern',
    'minor',
    'New PhD student, started Fall 2025. Young, enthusiastic, overwhelmed. Kate helps with sample prep, remembers being him three years ago. Patricia noticed Kate is good mentor.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- OTHER ADDITIONAL CHARACTERS
-- =====================================================

INSERT INTO characters (
    full_name, preferred_name, birthday, age,
    occupation, character_type, notes,
    created_at, updated_at
) VALUES
(
    'Sarah Goldstein', 'Sarah Goldstein', '1987-01-01', 38,
    'Marketing executive',
    'minor',
    'Marcus Chen''s ex-wife. Met at wedding summer 2015. Smart, ambitious, understood demanding careers. Married October 2016, divorced 2019. Had affair with coworker early 2018. Both were workaholics using each other to feel less guilty about work.',
    datetime('now'), datetime('now')
),
(
    'Steve Thomas', 'Steve', '1991-01-01', 34,
    'Business professional (client-facing, travels extensively)',
    'secondary',
    'Alice''s husband. Northwestern MBA. Met Alice at UIUC bar January 2016 (visiting friends). Long-distance first year while finishing MBA. Engaged December 2017, married June 2018. Father to Foster (7) and Robby (5). Travels 3-4 days/week for work. Earned Alice''s complete trust during PPD crisis.',
    datetime('now'), datetime('now')
),
(
    'Ms. Rodriguez', 'Ms. Rodriguez', NULL, NULL,
    'High school teacher (Kate''s time)',
    'minor',
    'Kate''s Environmental Science teacher in high school. Gave Kate field guides as gift. Saw Kate and understood her. Inscription in Kate''s senior yearbook (lost when Kate donated yearbooks). One of few people who saw Kate''s potential early.',
    datetime('now'), datetime('now')
),
(
    'Ahmad', 'Ahmad', NULL, NULL,
    'PhD student (Year 5), Environmental Engineering, Northwestern',
    'minor',
    'Kate''s office mate since she started. Environmental modeling focus. Friendly, occasionally complains about advisor. Kate listens but doesn''t share her own experiences. Comfortable low-intensity office mate relationship.',
    datetime('now'), datetime('now')
),
(
    'Diane', 'Diane', NULL, NULL,
    'Administrative Coordinator, Environmental Engineering Dept, Northwestern',
    'minor',
    'Manages logistics for all PhD students. Kate interacts for room reservations, reimbursements, forms. Efficient, professional. Knows Kate is quiet but capable. Helps Kate navigate administrative requirements.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- LOCATIONS TABLE - ADDITIONS
-- =====================================================

INSERT INTO locations (
    name, address_street, address_city, address_state, location_type, ownership_status, notes,
    created_at, updated_at
) VALUES
(
    'Hartwell Estate',
    'Sheridan Road',
    'Kenilworth',
    'IL',
    'residential_single_family',
    'owned',
    'Emma Hartwell''s childhood home. 6-bedroom lakefront estate. Purchased 1989 for $1.2M, now worth $4M+. Old money WASP aesthetic - pristine, cold, feels like museum. Pool house. Everything looks perfect, feels empty.',
    datetime('now'), datetime('now')
),
(
    'Caroline Hartwell-Montgomery House',
    NULL,
    'Glencoe',
    'IL',
    'residential_single_family',
    'owned',
    'Caroline (Emma''s sister) and Stratton''s home. $2.5M estate. Christmas gatherings here to show off grandchildren.',
    datetime('now'), datetime('now')
),
(
    'Alice & Steve Thomas House',
    NULL,
    'Wilmette',
    'IL',
    'residential_single_family',
    'owned',
    'Alice and Steve''s family home. Purchased October 2020. 4 bedrooms, 2.5 baths, ~2,200 sq ft. Good school district was major factor. Backyard for kids. Basement playroom. Kate will meet Paul''s family here eventually.',
    datetime('now'), datetime('now')
),
(
    'DDB Chicago Office',
    NULL,
    'Chicago',
    'IL',
    'commercial_office',
    NULL,
    'River North location. Major global advertising agency, ~400 employees in Chicago. Alice works here 3 days/week (hybrid). Account management, creative, media planning.',
    datetime('now'), datetime('now')
),
(
    'Danny O''Brien House',
    NULL,
    'Chicago',
    'IL',
    'residential_single_family',
    'owned',
    'Danny and Joan O''Brien''s house in Beverly neighborhood. Extended O''Brien family gatherings sometimes held here.',
    datetime('now'), datetime('now')
),
(
    'Sean O''Brien House',
    NULL,
    'Chicago',
    'IL',
    'residential_single_family',
    'owned',
    'Sean and Maria O''Brien''s house in Bridgeport neighborhood. Sean built it himself with family help.',
    datetime('now'), datetime('now')
),
(
    'Marie Brennan House',
    NULL,
    'Oak Park',
    'IL',
    'residential_single_family',
    'owned',
    'Marie and James Brennan''s house. Same house since 1993. Was two blocks from Ashley''s Austin home (across Austin Blvd). Frequent childhood visits.',
    datetime('now'), datetime('now')
),
(
    'Loyola University Chicago',
    '1032 W Sheridan Rd',
    'Chicago',
    'IL',
    'educational_university',
    NULL,
    'Where Ashley and Emma met freshman year 2014. Both graduated 2018. Rogers Park/Edgewater campus. Ashley''s formative college experience.',
    datetime('now'), datetime('now')
),
(
    'New Trier High School',
    '385 Winnetka Ave',
    'Winnetka',
    'IL',
    'educational_high_school',
    NULL,
    'Emma''s high school (2010-2014). Wealthy North Shore public school, excellent but pressure-cooker. Where Emma started drinking at parties junior year.',
    datetime('now'), datetime('now')
),
(
    'Niles North High School',
    '9800 Lawler Ave',
    'Skokie',
    'IL',
    'educational_high_school',
    NULL,
    'Alice''s high school (2008-2012). Volleyball team, student council, prom committee. Where she discovered she was good at sales through mall retail job.',
    datetime('now'), datetime('now')
),
(
    'Illinois State Water Survey',
    '2204 Griffith Dr',
    'Champaign',
    'IL',
    'research_facility',
    NULL,
    'Dr. Lisa Wu''s workplace. Kate''s external committee member works here. Applied research on Great Lakes water quality.',
    datetime('now'), datetime('now')
);

-- =====================================================
-- RELATIONSHIPS TABLE - ADDITIONS
-- Schema: Uses character_a_id and character_b_id (foreign keys)
-- Need to lookup character IDs from full_name
-- =====================================================

INSERT INTO relationships (
    character_a_id, character_b_id, relationship_type, relationship_subtype,
    start_date, end_date, status, notes,
    created_at, updated_at
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Emma Hartwell'),
    (SELECT id FROM characters WHERE full_name = 'Charles Harrison Hartwell III'),
    'family', 'father_daughter', NULL, NULL, 'active',
    'Dysfunctional relationship. Charles dismissive, controlling through money. Emma craves his approval, hates herself for it.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Emma Hartwell'),
    (SELECT id FROM characters WHERE full_name = 'Victoria Lancaster Hartwell'),
    'family', 'mother_daughter', NULL, NULL, 'active',
    'Dysfunctional. Victoria confides inappropriately, uses Emma as emotional support. Simultaneously loving and manipulative.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Emma Hartwell'),
    (SELECT id FROM characters WHERE full_name = 'Caroline Victoria Hartwell-Montgomery'),
    'family', 'siblings', NULL, NULL, 'active',
    'Competitive but occasionally allied against parents. Caroline was golden child. Can''t quite be honest with each other.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Patrick O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Daniel Francis O''Brien'),
    'family', 'siblings', NULL, NULL, 'active',
    'Two of three O''Brien brothers. Constant banter, arguing about everything. Deep loyalty underneath bickering.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Patrick O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Sean Michael O''Brien'),
    'family', 'siblings', NULL, NULL, 'active',
    'Patrick is middle brother, Sean is youngest. Sean is family peacemaker.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    (SELECT id FROM characters WHERE full_name = 'Steve Thomas'),
    'romantic_partner', 'married', '2018-06-01', NULL, 'active',
    'Strong partnership built on mutual respect and shared goals. Steve''s travel has been consistent since before marriage. Steve earned Alice''s complete trust during PPD crisis.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Patricia Chen'),
    'professional', 'advisor_advisee', '2022-09-06', NULL, 'active',
    'Primary dissertation advisor. Demanding but supportive. High standards. Kate respects her deeply, slightly fears her.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Michael Brennan'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'Committee member. Practical questions about implementation.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Sarah Kline'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'Committee member. Chemistry expertise. Toughest questioner about data quality.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. James Rodriguez'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'Committee member. Statistics expertise. Most comfortable after Patricia.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. Lisa Wu'),
    'professional', 'committee_member', '2024-04-21', NULL, 'active',
    'External committee member. Applied research perspective.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    (SELECT id FROM characters WHERE full_name = 'Dr. David Chen'),
    'professional', 'lab_colleague', '2022-09-01', NULL, 'active',
    'Postdoc who trained Kate on equipment. Go-to for troubleshooting. Collaborative on papers. Represents successful professional relationship for Kate.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Marcus Chen'),
    (SELECT id FROM characters WHERE full_name = 'Sarah Goldstein'),
    'ex_partner', 'ex_spouse', '2016-10-01', '2019-01-01', 'ended',
    'Met at wedding summer 2015. Married October 2016. Both workaholics. Sarah had affair early 2018. Divorce finalized 2019.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Ashley O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Sarah Brennan'),
    'family', 'cousins', NULL, NULL, 'active',
    'Closest cousin to Ashley - best friend level. Grew up two blocks apart. Bridesmaid. Talk/text daily.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Ashley O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Erin O''Brien'),
    'family', 'cousins', NULL, NULL, 'active',
    'Close cousin, friend as well as family. Similar age and personality. Bridesmaid. Meet for drinks/brunch.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Ashley O''Brien'),
    (SELECT id FROM characters WHERE full_name = 'Grace O''Brien Martinez'),
    'family', 'cousins', NULL, NULL, 'active',
    'Close cousin, same age, similar values. Bridesmaid. Meet for coffee monthly.',
    datetime('now'), datetime('now');

-- =====================================================
-- TIMELINE_EVENTS TABLE - ADDITIONS
-- Schema: Uses event_title and location_id
-- =====================================================

INSERT INTO timeline_events (
    event_date, event_title, event_description, location_id, importance,
    created_at, updated_at
) VALUES
-- Academic milestones (no specific location in database)
('2024-04-21', 'Kate comprehensive exam', 'Kate passed comprehensive exam. Three-hour oral defense. Committee of five grilled her. Patricia toughest questioner. Major milestone - now ABD.', NULL, 'high', datetime('now'), datetime('now')),
('2025-10-15', 'Kate proposal defense', 'Kate defended detailed research plan for remaining PhD years. Committee approved with minor revisions.', NULL, 'high', datetime('now'), datetime('now')),

-- Conference presentations (no specific location in database)
('2023-12-13', 'Kate AGU 2023 poster', 'Kate''s first major conference. Poster presentation on microplastics and PFAS in Chicago drinking water. San Francisco, Moscone Center. Overwhelmed by scale (20,000+ attendees). Skipped AGU student mixer.', NULL, 'medium', datetime('now'), datetime('now')),
('2024-04-09', 'Kate Illinois Water Conference', 'Kate presented at regional conference at UIUC (her undergrad). Connected with Dr. Lisa Wu who later joined committee. Strange being back at undergrad campus.', NULL, 'medium', datetime('now'), datetime('now')),
('2024-11-18', 'Kate SETAC oral presentation', 'Kate''s first major oral at national conference. Portland, OR. Very nervous but presented clearly. Tough Q&A, handled well. Skipped conference banquet and EPA mixer.', NULL, 'medium', datetime('now'), datetime('now')),

-- Hartwell/O'Brien family events (no specific location in database)
('2015-10-01', 'Ashley O''Brien family dinner', 'Emma met O''Brien family. Patrick: "So you''re the friend Ashley won''t shut up about." Emma unused to warmth, volume, casual affection.', NULL, 'low', datetime('now'), datetime('now')),
('2016-03-15', 'Emma O''Brien spring break', 'Emma joined O''Brien family vacation to Gulf Shores. First family vacation she actually enjoyed. Cried on last night.', NULL, 'low', datetime('now'), datetime('now')),

-- Alice/Steve milestones
('2016-01-15', 'Alice and Steve meet', 'Alice met Steve at bar near UIUC campus. He was Northwestern MBA student visiting friends. Started dating casually.', NULL, 'medium', datetime('now'), datetime('now')),
('2017-12-15', 'Alice and Steve engaged', 'Romantic proposal in Chicago with Christmas lights.', NULL, 'medium', datetime('now'), datetime('now')),
('2018-06-15', 'Alice and Steve wedding', 'Elegant wedding, 150 people. Beautiful day.', NULL, 'high', datetime('now'), datetime('now'));

-- Alice moving to Wilmette - WITH location_id lookup
INSERT INTO timeline_events (
    event_date, event_title, event_description, location_id, importance,
    created_at, updated_at
)
SELECT
    '2020-10-15', 'Alice and Steve move to Wilmette',
    'Moved from Chicago to Wilmette. Needed more space with two kids. Better schools.',
    (SELECT id FROM locations WHERE name = 'Alice & Steve Thomas House'),
    'medium',
    datetime('now'), datetime('now');

-- =====================================================
-- POSSESSIONS TABLE - ADDITIONS
-- Schema: Uses owner_id (foreign key to characters)
-- =====================================================

INSERT INTO possessions (
    owner_id, item_name, description, acquisition_date, acquisition_method,
    current_location, sentimental_value, notes,
    created_at, updated_at
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Hario gooseneck kettle',
    'Paired with Chemex for pour-over coffee ritual',
    NULL, 'purchased',
    'Paul''s house (as of Jan 2026)', 'medium',
    'Part of sacred morning coffee ritual. Precise water temperature.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Baratza Encore grinder',
    'Burr coffee grinder for consistent grind',
    NULL, 'purchased',
    'Paul''s house (as of Jan 2026)', 'medium',
    'Grinds beans fresh each morning. Exact amount every time.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Running medals collection',
    '9 race medals including one marathon. Proof of finishing things.',
    '2014-05-01', 'earned',
    'Kate''s apartment bedroom wall', 'high',
    'Only things on walls besides watershed map. Naperville Half (2014), Chicago Marathon (2016 - 3:54:32, cried at finish), various 10Ks and halfs.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Mike''s UIUC sweatshirt',
    'Navy blue, Fighting Illini, worn soft. Mike gave for 18th birthday. Small bleach stain, missing drawstring.',
    '2011-03-15', 'gift',
    'Kate''s apartment bottom drawer', 'high',
    'Wore constantly freshman year. Evidence Mike once paid attention. Can''t throw away.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Nancy Drew collection',
    '28 books, yellowed pages. Ages 8-12. First fictional character Kate identified with.',
    NULL, 'childhood',
    'Mom''s house Naperville (closet)', 'medium',
    'Some volumes falling apart. Can''t throw away despite not rereading.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Harry Potter series',
    'All 7, original US editions. HP1 extremely worn (read 15+ times). Marginalia in pencil.',
    NULL, 'childhood',
    'Mom''s house Naperville (closet)', 'medium',
    'Connection to period when she read for joy, not escape.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Silent Spring book',
    'Rachel Carson. High school Environmental Science class (2009). Changed her life direction. Yellow highlighter throughout. Notes from 17-year-old Kate.',
    '2009-01-01', 'class',
    'Mom''s house Naperville (closet)', 'high',
    'Reminder of why she chose environmental engineering path.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Science fair trophies',
    'Three trophies from middle school. 2nd place (6th), 1st place (7th & 8th). Water quality focused.',
    '2005-01-01', 'earned',
    'Mom''s house Naperville (childhood bedroom)', 'medium',
    'Evidence 13-year-old Kate cared about water quality. Tarnished gold plastic. Too childish to display as adult, too meaningful to discard.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'MacBook Pro 2021',
    'Primary work laptop for dissertation writing',
    '2021-01-01', 'purchased',
    'Kate''s apartment desk', 'low',
    'With external monitor, wireless keyboard and mouse.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Great Lakes watershed map',
    'Framed. Only wall art in apartment besides running medals.',
    NULL, 'purchased',
    'Kate''s apartment living room wall', 'medium',
    'Evidence of professional identity in personal space.',
    datetime('now'), datetime('now');

-- =====================================================
-- SCHEDULES TABLE - ADDITIONS
-- Schema: Uses character_id, schedule_name, days_of_week (JSON array)
-- =====================================================

INSERT INTO schedules (
    character_id, schedule_name, days_of_week, start_time, end_time,
    location, is_mandatory, notes,
    created_at, updated_at
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Lab meeting',
    '["thursday"]',
    '14:00', '16:00',
    'Tech Building, Northwestern', 1,
    'Data presentations, troubleshooting, paper planning. Kate prepares carefully. Presents every 3-4 weeks.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Thursday coffee social',
    '["thursday"]',
    '15:00', '16:00',
    'Chen Lab kitchen', 0,
    'Informal gathering. Patricia brings good coffee. Kate attends ~80% of time, stays 30-40 minutes, doesn''t stay for extended socializing.',
    datetime('now'), datetime('now');

-- =====================================================
-- CHARACTER_NEGATIVES TABLE - ADDITIONS
-- Schema: Uses character_id, negative_behavior, strength
-- =====================================================

INSERT INTO character_negatives (
    character_id, negative_behavior, category, strength, exceptions, notes,
    created_at, updated_at
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Does NOT attend student mixers at conferences',
    'social', 'strong', NULL,
    'Skipped AGU student mixer (too anxiety-inducing). Too overwhelming, too many people.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Does NOT stay at networking events longer than 45-60 minutes',
    'social', 'strong',
    'If required for specific professional reason',
    'Maximum capacity. Leaves after reasonable effort. Did minimum necessary, not maximum beneficial.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Does NOT attend conference banquets or social dinners',
    'social', 'strong',
    'Required for committee or advisor',
    'Skipped SETAC conference banquet, EPA mixer. Patricia frustrated but Kate "just can''t."',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Kate Morrison'),
    'Does NOT pursue connections made at conferences',
    'social', 'preference',
    'If professional collaboration clearly valuable',
    'Exchanged cards with 8-10 people, followed up with 4-5 briefly. Pattern of letting connections drift.',
    datetime('now'), datetime('now');

-- =====================================================
-- EDUCATION TABLE - ADDITIONS
-- Schema: Uses character_id, degree_type, field_of_study
-- Removed columns: advisor, gpa (don't exist in current schema)
-- =====================================================

INSERT INTO education (
    character_id, institution, degree_type, field_of_study, start_year, end_year,
    honors, notes,
    created_at, updated_at
)
SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    'University of Illinois Urbana-Champaign', 'B.A.', 'Advertising',
    2012, 2016,
    'Dean''s List, cum laude',
    'Minor in Psychology. Ad Club vice president senior year. Study abroad London Spring 2015. Met Steve senior year.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    NULL, NULL, 'Google Ads certification',
    NULL, NULL, NULL,
    'Industry certification for advertising work.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Alice Thomas'),
    NULL, NULL, 'Facebook Blueprint certification',
    NULL, NULL, NULL,
    'Industry certification for advertising work.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Steve Thomas'),
    'Northwestern University', 'MBA', 'Business',
    2015, 2017,
    'Kellogg School',
    'Met Alice while visiting UIUC friends January 2016.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Erin O''Brien'),
    'Unknown', 'BSN', 'Nursing',
    NULL, NULL, NULL,
    'Works at Northwestern Memorial Hospital.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Michael O''Brien'),
    'Loyola University Chicago', 'M.D.', 'Medicine',
    2023, 2027,
    'Stritch School of Medicine',
    'Year 3 medical student. Sean and Maria''s son.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Katie Brennan'),
    'University of Chicago', 'MSW', 'Social Work',
    2022, 2024, NULL,
    'Marie and James''s daughter. Starting social work career.',
    datetime('now'), datetime('now')
UNION ALL SELECT
    (SELECT id FROM characters WHERE full_name = 'Thomas Brennan'),
    'Unknown', 'B.S.', 'Computer Science',
    NULL, NULL, NULL,
    'Software engineer. Marie and James''s son. Engaged to Jennifer.',
    datetime('now'), datetime('now');

-- =====================================================
-- END OF TRANSFORMED SEED DATA
-- =====================================================
