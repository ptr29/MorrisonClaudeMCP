-- =====================================================
-- Kate Morrison Canonical Facts Database - ADDITIONS
-- Second Pass: Additional Entries Found
-- Generated: 2026-01-15
-- =====================================================
-- This file contains additional entries discovered in second
-- pass through project files. Run after main seed file.
-- =====================================================

-- =====================================================
-- CHARACTERS TABLE - ADDITIONS
-- =====================================================

-- =====================================================
-- HARTWELL FAMILY (Emma's family)
-- =====================================================

INSERT INTO characters (
    name, preferred_name, birth_date, age, gender,
    physical_description, occupation, residence_location, category, notes
) VALUES
-- Charles Hartwell III (Emma's father)
(
    'Charles Harrison Hartwell III', 'Charles', '1963-01-01', 62, 'male',
    'Classic corporate lawyer appearance, commanding presence',
    'Managing Partner, corporate litigation law firm',
    'Kenilworth, IL',
    'supporting',
    'Emma''s father. Yale undergrad, Harvard Law. Income $800K+/year. Classic narcissist - charming in public, cruel in private. Controls through money. Has had affairs. Wanted a son.'
),
-- Victoria Hartwell (Emma's mother)
(
    'Victoria Lancaster Hartwell', 'Victoria', '1966-01-01', 59, 'female',
    'North Shore old money appearance, polished, anxious',
    'Philanthropist, charity board member',
    'Kenilworth, IL',
    'supporting',
    'Emma''s mother. North Shore old money family, debutante. Passive-aggressive perfectionist. Self-medicates with Xanax and wine. Enables Charles''s behavior. Confides in Emma inappropriately.'
),
-- Caroline Hartwell-Montgomery (Emma's sister)
(
    'Caroline Victoria Hartwell-Montgomery', 'Caroline', '1994-01-01', 31, 'female',
    'Polished, appears to have it together, actually miserable',
    'Homemaker, Junior League, charity boards',
    'Glencoe, IL',
    'supporting',
    'Emma''s older sister (3 years older). Married Stratton Montgomery III. Two sons Harrison (7) and Charles (5). $2.5M home. Followed the script perfectly but actually unhappy. Day drinking normalized.'
),
-- Stratton Montgomery III
(
    'Stratton Montgomery III', 'Stratton', '1992-01-01', 33, 'male',
    'Investment banker appearance',
    'Investment banker',
    'Glencoe, IL',
    'supporting',
    'Caroline''s husband. Works 80-hour weeks. Father to Harrison and Charles. Old money background.'
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Patrick's Brothers
-- =====================================================

INSERT INTO characters (
    name, preferred_name, birth_date, age, gender,
    physical_description, occupation, residence_location, category, notes
) VALUES
-- Danny O'Brien (Patrick's older brother)
(
    'Daniel Francis O''Brien', 'Danny', '1962-01-01', 63, 'male',
    'Gruff cop appearance, intense presence',
    'Retired CPD Sergeant',
    'Beverly neighborhood, Chicago',
    'supporting',
    'Patrick''s older brother. CPD career 1982-2020 (38 years). Classic cop - gruff, opinionated, loves arguing politics. White Sox fanatic, season tickets. Drinks whiskey, tells long police stories. Knights of Columbus member. Calls Ashley "Red".'
),
-- Joan O'Brien (Danny's wife)
(
    'Joan Murphy O''Brien', 'Joan', '1963-01-01', 62, 'female',
    'Calm, organized presence',
    'Retired elementary school principal',
    'Beverly neighborhood, Chicago',
    'supporting',
    'Danny''s wife since 1985. The calm to Danny''s storm. Excellent cook - Irish soda bread, shepherd''s pie. Close with Maureen. Book club member, garden enthusiast. Manages Danny''s retirement schedule.'
),
-- Sean O'Brien (Patrick's younger brother)
(
    'Sean Michael O''Brien', 'Sean', '1969-01-01', 56, 'male',
    'Laid-back, easy-going appearance',
    'Union carpenter, Local 1 Chicago',
    'Bridgeport neighborhood, Chicago',
    'supporting',
    'Patrick''s younger brother. Union carpenter 35+ years. Built his own house. Most laid-back of three brothers. Plays guitar at family gatherings. Family peacemaker. Calls Ashley "Ash-tray". Making wooden gift for her wedding.'
),
-- Maria O'Brien (Sean's wife)
(
    'Maria Gonzalez O''Brien', 'Maria', '1971-01-01', 54, 'female',
    'Warm, welcoming presence',
    'Dental hygienist',
    'Bridgeport neighborhood, Chicago',
    'supporting',
    'Sean''s wife since 1992. Mexican-American. Brings cultural diversity to Irish clan. Makes tamales for Christmas Eve (now family tradition). Bilingual. Gets along with everyone.'
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Danny's Children (Ashley's Cousins)
-- =====================================================

INSERT INTO characters (
    name, preferred_name, birth_date, age, gender,
    physical_description, occupation, residence_location, category, notes
) VALUES
(
    'Meghan O''Brien Callahan', 'Meghan', '1987-01-01', 38, 'female',
    NULL,
    'High school English teacher',
    'Western Springs, IL',
    'minor',
    'Danny''s oldest daughter. Married to Tom Callahan (40). Three children: Danny Jr. (15), Bridget (13), Finn (10). Hosting Thanksgiving 2025. Ashley admires her, age gap means not super close.'
),
(
    'Daniel O''Brien Jr.', 'DJ', '1990-01-01', 35, 'male',
    NULL,
    'Chicago firefighter',
    'Mount Greenwood, Chicago',
    'minor',
    'Danny''s son. Named after father, goes by DJ. Carries on family firefighter tradition. Married to Rachel (33), two kids Connor (8), Riley (5). Danny Sr. and Patrick very proud.'
),
(
    'Kevin O''Brien', 'Kevin', '1993-01-01', 32, 'male',
    NULL,
    'Accountant',
    'Oak Park area, IL',
    'minor',
    'Danny''s son. Accountant at mid-size firm. Married to Lauren (31), daughter Nora (3). Quieter than other O''Briens. Does taxes for family members.'
),
(
    'Erin O''Brien', 'Erin', '1996-01-01', 29, 'female',
    NULL,
    'Nurse at Northwestern Memorial',
    'Lincoln Park, Chicago',
    'minor',
    'Danny''s youngest. Single, dating on and off. Fun, social, closest to Ashley in personality. Bridesmaid in Ashley''s wedding. They text regularly, share dating stories, meet for drinks/brunch.'
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Sean's Children
-- =====================================================

INSERT INTO characters (
    name, preferred_name, birth_date, age, gender,
    physical_description, occupation, residence_location, category, notes
) VALUES
(
    'Patrick O''Brien Jr.', 'Patrick Jr.', '1994-01-01', 31, 'male',
    NULL,
    'Electrician',
    'Bridgeport neighborhood, Chicago',
    'minor',
    'Sean''s oldest. Named after Uncle Patrick. Works with cousin Brendan sometimes. Married to Amy (30), two kids Sean Jr. (6), Ella (3). Solid guy, family-oriented, quiet.'
),
(
    'Grace O''Brien Martinez', 'Grace', '1997-01-01', 28, 'female',
    NULL,
    'Social worker at DCFS',
    'Pilsen, Chicago',
    'minor',
    'Sean''s daughter. Married to Carlos Martinez (29). Passionate about social justice. Close with Ashley - same age, similar values. Bridesmaid in wedding. Balances Mexican and Irish heritage comfortably.'
),
(
    'Michael O''Brien', 'Mikey', '2000-01-01', 25, 'male',
    NULL,
    'Medical student, Loyola Stritch (3rd year)',
    'Chicago area',
    'minor',
    'Sean''s son. Goes by "Mikey" to distinguish from other Michaels. Smart, ambitious. Family very proud. Single, focused on school.'
),
(
    'Ana O''Brien', 'Ana', '2003-01-01', 22, 'female',
    NULL,
    'Marketing (entry-level)',
    'Wicker Park, Chicago',
    'minor',
    'Sean''s youngest. Recent DePaul grad (2024). Lives with roommates. Youngest cousin on O''Brien side. Finding her way in adult life. Ashley offers career advice when asked.'
);

-- =====================================================
-- O'BRIEN EXTENDED FAMILY - Maureen's Side (Murphy/Brennan)
-- =====================================================

INSERT INTO characters (
    name, preferred_name, birth_date, age, gender,
    physical_description, occupation, residence_location, category, notes
) VALUES
(
    'Marie Catherine Murphy Brennan', 'Marie', '1970-01-01', 55, 'female',
    NULL,
    'Nurse Practitioner at Oak Park clinic',
    'Oak Park, IL',
    'minor',
    'Maureen''s younger sister (only sibling). Oak Park native, never left. Close with Maureen - talk daily, see each other constantly. More reserved than Maureen but still warm. Thoughtful, reads constantly. Lived two blocks from Ashley growing up.'
),
(
    'James Brennan', 'James', '1968-01-01', 57, 'male',
    NULL,
    'High school history teacher at Oak Park-River Forest HS',
    'Oak Park, IL',
    'minor',
    'Marie''s husband since 1993. Intellectual, reads history books, watches documentaries. Coaches basketball. Gets along with O''Brien brothers despite different temperament.'
),
(
    'Sarah Brennan', 'Sarah', '1996-01-01', 29, 'female',
    NULL,
    'Elementary school teacher',
    'Logan Square, Chicago',
    'minor',
    'Marie''s oldest daughter. Single, dating casually. Closest cousin to Ashley - best friend level. Grew up two blocks apart (practically neighbors). Bridesmaid. Talk/text daily, know everything about each other.'
),
(
    'Thomas Brennan', 'Tom', '1998-01-01', 27, 'male',
    NULL,
    'Software engineer',
    'West Loop, Chicago',
    'minor',
    'Marie''s son. Engaged to Jennifer (26), wedding Fall 2026. Quieter than other cousins, more introverted.'
),
(
    'Katie Brennan', 'Katie', '2001-01-01', 24, 'female',
    NULL,
    'Social worker (starting career)',
    'Oak Park, IL (temporarily at parents)',
    'minor',
    'Marie''s youngest. Recent MSW from UChicago. Lives at home temporarily while job searching. Sweet, idealistic, passionate about helping people.'
);

-- =====================================================
-- NORTHWESTERN ACADEMIC - Kate's Committee and Lab
-- =====================================================

INSERT INTO characters (
    name, preferred_name, birth_date, age, gender,
    physical_description, occupation, residence_location, category, notes
) VALUES
(
    'Dr. Patricia Chen', 'Patricia', '1979-01-01', 46, 'female',
    'Mid-40s, Chinese-American, organized but casual academic appearance',
    'Associate Professor, Civil & Environmental Engineering, Northwestern',
    'Chicago area',
    'secondary',
    'Kate''s dissertation advisor. Recruited to Northwestern for Great Lakes research access. Research: microplastics and PFAS in drinking water. Management style: demanding but supportive, high standards, direct communication. Committee chair.'
),
(
    'Dr. Michael Brennan', 'Michael Brennan', '1967-01-01', 58, 'male',
    'Late 50s, practical academic',
    'Professor, Civil & Environmental Engineering, Northwestern',
    'Chicago area',
    'minor',
    'Kate''s committee member #2. Expertise: drinking water treatment, infrastructure, regulatory compliance. Practical, industry-focused, asks operational questions. Kate took his Water Treatment Engineering class Year 1.'
),
(
    'Dr. Sarah Kline', 'Sarah Kline', '1988-01-01', 37, 'female',
    'Late 30s, precise demeanor',
    'Associate Professor, Chemistry, Northwestern',
    'Chicago area',
    'minor',
    'Kate''s committee member #3. Expertise: analytical chemistry, environmental contaminants, mass spectrometry. Detail-oriented, methodologically rigorous. Toughest questioner about data quality. Kate audited her Advanced Analytical Chemistry seminar.'
),
(
    'Dr. James Rodriguez', 'James Rodriguez', '1984-01-01', 41, 'male',
    'Early 40s, encouraging demeanor',
    'Associate Professor, Civil & Environmental Engineering, Northwestern',
    'Chicago area',
    'minor',
    'Kate''s committee member #4. Expertise: contaminant transport modeling, environmental fate, statistics. Theoretical, big-picture thinker. Most comfortable committee member after Patricia. Kate took his Contaminant Fate & Transport class Year 1.'
),
(
    'Dr. Lisa Wu', 'Lisa Wu', '1970-01-01', 55, 'female',
    'Mid-50s, applied research focus',
    'Senior Research Scientist, Illinois State Water Survey',
    'Champaign, IL',
    'minor',
    'Kate''s external committee member #5. Expertise: Great Lakes water quality, applied research, policy interface. Collaborative, supportive of Kate''s work. Suggested Kate consider academic job market.'
),
(
    'Dr. David Chen', 'David Chen', '1993-01-01', 32, 'male',
    'Early 30s, lean, patient demeanor',
    'Postdoctoral Researcher, Chen Lab, Northwestern',
    'Chicago area',
    'minor',
    'Postdoc in Patricia Chen''s lab (no relation to Patricia). MIT PhD. Research: PFAS transport mechanisms. Trained Kate on analytical equipment (GC-MS/MS, Py-GC-MS). Patient instructor. Go-to for method troubleshooting. Collaborative on papers. Professional relationship only.'
),
(
    'Jessica Okonkwo', 'Jessica', '1997-01-01', 28, 'female',
    'Nigerian-American, warm, extroverted',
    'PhD student (Year 6), Chen Lab, Northwestern',
    'Chicago area',
    'minor',
    'Fellow PhD student. Year 6, defending Spring 2026. Rice undergrad. Research: nanomaterial fate in aquatic systems. Warm, extroverted, social organizer. Tried to befriend Kate, eventually accepted cordial but distant dynamic. Kate couldn''t provide friend Kate needed.'
),
(
    'Rashid Ahmed', 'Rashid', '2000-01-01', 25, 'male',
    'Young, eager appearance',
    'PhD student (Year 1), Chen Lab, Northwestern',
    'Chicago area',
    'minor',
    'New PhD student, started Fall 2025. Young, enthusiastic, overwhelmed. Kate helps with sample prep, remembers being him three years ago. Patricia noticed Kate is good mentor.'
);

-- =====================================================
-- OTHER ADDITIONAL CHARACTERS
-- =====================================================

INSERT INTO characters (
    name, preferred_name, birth_date, age, gender,
    physical_description, occupation, residence_location, category, notes
) VALUES
(
    'Sarah Goldstein', 'Sarah Goldstein', '1987-01-01', 38, 'female',
    NULL,
    'Marketing executive',
    'Chicago area',
    'minor',
    'Marcus Chen''s ex-wife. Met at wedding summer 2015. Smart, ambitious, understood demanding careers. Married October 2016, divorced 2019. Had affair with coworker early 2018. Both were workaholics using each other to feel less guilty about work.'
),
(
    'Steve Thomas', 'Steve', '1991-01-01', 34, 'male',
    NULL,
    'Business professional (client-facing, travels extensively)',
    'Wilmette, IL',
    'secondary',
    'Alice''s husband. Northwestern MBA. Met Alice at UIUC bar January 2016 (visiting friends). Long-distance first year while finishing MBA. Engaged December 2017, married June 2018. Father to Foster (7) and Robby (5). Travels 3-4 days/week for work. Earned Alice''s complete trust during PPD crisis.'
),
(
    'Ms. Rodriguez', 'Ms. Rodriguez', NULL, NULL, 'female',
    NULL,
    'High school teacher (Kate''s time)',
    'Naperville area',
    'minor',
    'Kate''s Environmental Science teacher in high school. Gave Kate field guides as gift. Saw Kate and understood her. Inscription in Kate''s senior yearbook (lost when Kate donated yearbooks). One of few people who saw Kate''s potential early.'
),
(
    'Ahmad', 'Ahmad', NULL, NULL, 'male',
    NULL,
    'PhD student (Year 5), Environmental Engineering, Northwestern',
    'Chicago area',
    'minor',
    'Kate''s office mate since she started. Environmental modeling focus. Friendly, occasionally complains about advisor. Kate listens but doesn''t share her own experiences. Comfortable low-intensity office mate relationship.'
),
(
    'Diane', 'Diane', NULL, NULL, 'female',
    NULL,
    'Administrative Coordinator, Environmental Engineering Dept, Northwestern',
    'Evanston area',
    'minor',
    'Manages logistics for all PhD students. Kate interacts for room reservations, reimbursements, forms. Efficient, professional. Knows Kate is quiet but capable. Helps Kate navigate administrative requirements.'
);

-- =====================================================
-- LOCATIONS TABLE - ADDITIONS
-- =====================================================

INSERT INTO locations (
    name, address, city, state, type, ownership_status, notes
) VALUES
(
    'Hartwell Estate',
    'Sheridan Road',
    'Kenilworth',
    'IL',
    'residential_single_family',
    'owned',
    'Emma Hartwell''s childhood home. 6-bedroom lakefront estate. Purchased 1989 for $1.2M, now worth $4M+. Old money WASP aesthetic - pristine, cold, feels like museum. Pool house. Everything looks perfect, feels empty.'
),
(
    'Caroline Hartwell-Montgomery House',
    NULL,
    'Glencoe',
    'IL',
    'residential_single_family',
    'owned',
    'Caroline (Emma''s sister) and Stratton''s home. $2.5M estate. Christmas gatherings here to show off grandchildren.'
),
(
    'Alice & Steve Thomas House',
    NULL,
    'Wilmette',
    'IL',
    'residential_single_family',
    'owned',
    'Alice and Steve''s family home. Purchased October 2020. 4 bedrooms, 2.5 baths, ~2,200 sq ft. Good school district was major factor. Backyard for kids. Basement playroom. Kate will meet Paul''s family here eventually.'
),
(
    'DDB Chicago Office',
    NULL,
    'Chicago',
    'IL',
    'commercial_office',
    NULL,
    'River North location. Major global advertising agency, ~400 employees in Chicago. Alice works here 3 days/week (hybrid). Account management, creative, media planning.'
),
(
    'Danny O''Brien House',
    NULL,
    'Chicago',
    'IL',
    'residential_single_family',
    'owned',
    'Danny and Joan O''Brien''s house in Beverly neighborhood. Extended O''Brien family gatherings sometimes held here.'
),
(
    'Sean O''Brien House',
    NULL,
    'Chicago',
    'IL',
    'residential_single_family',
    'owned',
    'Sean and Maria O''Brien''s house in Bridgeport neighborhood. Sean built it himself with family help.'
),
(
    'Marie Brennan House',
    NULL,
    'Oak Park',
    'IL',
    'residential_single_family',
    'owned',
    'Marie and James Brennan''s house. Same house since 1993. Was two blocks from Ashley''s Austin home (across Austin Blvd). Frequent childhood visits.'
),
(
    'Loyola University Chicago',
    '1032 W Sheridan Rd',
    'Chicago',
    'IL',
    'educational_university',
    NULL,
    'Where Ashley and Emma met freshman year 2014. Both graduated 2018. Rogers Park/Edgewater campus. Ashley''s formative college experience.'
),
(
    'New Trier High School',
    '385 Winnetka Ave',
    'Winnetka',
    'IL',
    'educational_high_school',
    NULL,
    'Emma''s high school (2010-2014). Wealthy North Shore public school, excellent but pressure-cooker. Where Emma started drinking at parties junior year.'
),
(
    'Niles North High School',
    '9800 Lawler Ave',
    'Skokie',
    'IL',
    'educational_high_school',
    NULL,
    'Alice''s high school (2008-2012). Volleyball team, student council, prom committee. Where she discovered she was good at sales through mall retail job.'
),
(
    'Illinois State Water Survey',
    '2204 Griffith Dr',
    'Champaign',
    'IL',
    'research_facility',
    NULL,
    'Dr. Lisa Wu''s workplace. Kate''s external committee member works here. Applied research on Great Lakes water quality.'
);

-- =====================================================
-- RELATIONSHIPS TABLE - ADDITIONS
-- =====================================================

INSERT INTO relationships (
    character1_name, character2_name, relationship_type, relationship_subtype,
    start_date, end_date, status, notes
) VALUES
-- Hartwell family
('Emma Hartwell', 'Charles Harrison Hartwell III', 'family', 'father_daughter', NULL, NULL, 'active', 'Dysfunctional relationship. Charles dismissive, controlling through money. Emma craves his approval, hates herself for it.'),
('Emma Hartwell', 'Victoria Lancaster Hartwell', 'family', 'mother_daughter', NULL, NULL, 'active', 'Dysfunctional. Victoria confides inappropriately, uses Emma as emotional support. Simultaneously loving and manipulative.'),
('Emma Hartwell', 'Caroline Victoria Hartwell-Montgomery', 'family', 'siblings', NULL, NULL, 'active', 'Competitive but occasionally allied against parents. Caroline was golden child. Can''t quite be honest with each other.'),

-- O'Brien brothers
('Patrick O''Brien', 'Daniel Francis O''Brien', 'family', 'siblings', NULL, NULL, 'active', 'Two of three O''Brien brothers. Constant banter, arguing about everything. Deep loyalty underneath bickering.'),
('Patrick O''Brien', 'Sean Michael O''Brien', 'family', 'siblings', NULL, NULL, 'active', 'Patrick is middle brother, Sean is youngest. Sean is family peacemaker.'),

-- Alice and Steve
('Alice Thomas', 'Steve Thomas', 'romantic_partner', 'married', '2018-06-01', NULL, 'active', 'Strong partnership built on mutual respect and shared goals. Steve''s travel has been consistent since before marriage. Steve earned Alice''s complete trust during PPD crisis.'),

-- Kate's committee relationships
('Kate Morrison', 'Dr. Patricia Chen', 'professional', 'advisor_advisee', '2022-09-06', NULL, 'active', 'Primary dissertation advisor. Demanding but supportive. High standards. Kate respects her deeply, slightly fears her.'),
('Kate Morrison', 'Dr. Michael Brennan', 'professional', 'committee_member', '2024-04-21', NULL, 'active', 'Committee member. Practical questions about implementation.'),
('Kate Morrison', 'Dr. Sarah Kline', 'professional', 'committee_member', '2024-04-21', NULL, 'active', 'Committee member. Chemistry expertise. Toughest questioner about data quality.'),
('Kate Morrison', 'Dr. James Rodriguez', 'professional', 'committee_member', '2024-04-21', NULL, 'active', 'Committee member. Statistics expertise. Most comfortable after Patricia.'),
('Kate Morrison', 'Dr. Lisa Wu', 'professional', 'committee_member', '2024-04-21', NULL, 'active', 'External committee member. Applied research perspective.'),
('Kate Morrison', 'Dr. David Chen', 'professional', 'lab_colleague', '2022-09-01', NULL, 'active', 'Postdoc who trained Kate on equipment. Go-to for troubleshooting. Collaborative on papers. Represents successful professional relationship for Kate.'),

-- Marcus's ex-wife
('Marcus Chen', 'Sarah Goldstein', 'ex_partner', 'ex_spouse', '2016-10-01', '2019-01-01', 'ended', 'Met at wedding summer 2015. Married October 2016. Both workaholics. Sarah had affair early 2018. Divorce finalized 2019.'),

-- Ashley's close cousins
('Ashley O''Brien', 'Sarah Brennan', 'family', 'cousins', NULL, NULL, 'active', 'Closest cousin to Ashley - best friend level. Grew up two blocks apart. Bridesmaid. Talk/text daily.'),
('Ashley O''Brien', 'Erin O''Brien', 'family', 'cousins', NULL, NULL, 'active', 'Close cousin, friend as well as family. Similar age and personality. Bridesmaid. Meet for drinks/brunch.'),
('Ashley O''Brien', 'Grace O''Brien Martinez', 'family', 'cousins', NULL, NULL, 'active', 'Close cousin, same age, similar values. Bridesmaid. Meet for coffee monthly.');

-- =====================================================
-- TIMELINE_EVENTS TABLE - ADDITIONS
-- =====================================================

INSERT INTO timeline_events (
    event_date, event_name, event_description, location, importance
) VALUES
-- Academic milestones
('2024-04-21', 'Kate comprehensive exam', 'Kate passed comprehensive exam. Three-hour oral defense. Committee of five grilled her. Patricia toughest questioner. Major milestone - now ABD.', 'Northwestern University', 'high'),
('2025-10-15', 'Kate proposal defense', 'Kate defended detailed research plan for remaining PhD years. Committee approved with minor revisions.', 'Northwestern University', 'high'),

-- Conference presentations
('2023-12-13', 'Kate AGU 2023 poster', 'Kate''s first major conference. Poster presentation on microplastics and PFAS in Chicago drinking water. San Francisco, Moscone Center. Overwhelmed by scale (20,000+ attendees). Skipped AGU student mixer.', 'San Francisco, CA', 'medium'),
('2024-04-09', 'Kate Illinois Water Conference', 'Kate presented at regional conference at UIUC (her undergrad). Connected with Dr. Lisa Wu who later joined committee. Strange being back at undergrad campus.', 'UIUC, Urbana-Champaign', 'medium'),
('2024-11-18', 'Kate SETAC oral presentation', 'Kate''s first major oral at national conference. Portland, OR. Very nervous but presented clearly. Tough Q&A, handled well. Skipped conference banquet and EPA mixer.', 'Portland, OR', 'medium'),

-- Hartwell/O'Brien family events
('2015-10-01', 'Ashley O''Brien family dinner', 'Emma met O''Brien family. Patrick: "So you''re the friend Ashley won''t shut up about." Emma unused to warmth, volume, casual affection.', 'O''Brien family home', 'low'),
('2016-03-15', 'Emma O''Brien spring break', 'Emma joined O''Brien family vacation to Gulf Shores. First family vacation she actually enjoyed. Cried on last night.', 'Gulf Shores, Alabama', 'low'),

-- Alice/Steve milestones  
('2016-01-15', 'Alice and Steve meet', 'Alice met Steve at bar near UIUC campus. He was Northwestern MBA student visiting friends. Started dating casually.', 'Urbana-Champaign, IL', 'medium'),
('2017-12-15', 'Alice and Steve engaged', 'Romantic proposal in Chicago with Christmas lights.', 'Chicago', 'medium'),
('2018-06-15', 'Alice and Steve wedding', 'Elegant wedding, 150 people. Beautiful day.', 'Chicago area', 'high'),
('2020-10-15', 'Alice and Steve move to Wilmette', 'Moved from Chicago to Wilmette. Needed more space with two kids. Better schools.', 'Wilmette, IL', 'medium');

-- =====================================================
-- POSSESSIONS TABLE - ADDITIONS
-- =====================================================

INSERT INTO possessions (
    owner_name, item_name, description, acquisition_date, acquisition_method,
    current_location, sentimental_value, notes
) VALUES
-- Coffee equipment
('Kate Morrison', 'Hario gooseneck kettle', 'Paired with Chemex for pour-over coffee ritual', NULL, 'purchased', 'Paul''s house (as of Jan 2026)', 'medium', 'Part of sacred morning coffee ritual. Precise water temperature.'),
('Kate Morrison', 'Baratza Encore grinder', 'Burr coffee grinder for consistent grind', NULL, 'purchased', 'Paul''s house (as of Jan 2026)', 'medium', 'Grinds beans fresh each morning. Exact amount every time.'),

-- Running/achievements
('Kate Morrison', 'Running medals collection', '9 race medals including one marathon. Proof of finishing things.', '2014-05-01', 'earned', 'Kate''s apartment bedroom wall', 'high', 'Only things on walls besides watershed map. Naperville Half (2014), Chicago Marathon (2016 - 3:54:32, cried at finish), various 10Ks and halfs.'),

-- Sentimental items at mom''s house
('Kate Morrison', 'Mike''s UIUC sweatshirt', 'Navy blue, Fighting Illini, worn soft. Mike gave for 18th birthday. Small bleach stain, missing drawstring.', '2011-03-15', 'gift', 'Kate''s apartment bottom drawer', 'high', 'Wore constantly freshman year. Evidence Mike once paid attention. Can''t throw away.'),
('Kate Morrison', 'Nancy Drew collection', '28 books, yellowed pages. Ages 8-12. First fictional character Kate identified with.', NULL, 'childhood', 'Mom''s house Naperville (closet)', 'medium', 'Some volumes falling apart. Can''t throw away despite not rereading.'),
('Kate Morrison', 'Harry Potter series', 'All 7, original US editions. HP1 extremely worn (read 15+ times). Marginalia in pencil.', NULL, 'childhood', 'Mom''s house Naperville (closet)', 'medium', 'Connection to period when she read for joy, not escape.'),
('Kate Morrison', 'Silent Spring book', 'Rachel Carson. High school Environmental Science class (2009). Changed her life direction. Yellow highlighter throughout. Notes from 17-year-old Kate.', '2009-01-01', 'class', 'Mom''s house Naperville (closet)', 'high', 'Reminder of why she chose environmental engineering path.'),
('Kate Morrison', 'Science fair trophies', 'Three trophies from middle school. 2nd place (6th), 1st place (7th & 8th). Water quality focused.', '2005-01-01', 'earned', 'Mom''s house Naperville (childhood bedroom)', 'medium', 'Evidence 13-year-old Kate cared about water quality. Tarnished gold plastic. Too childish to display as adult, too meaningful to discard.'),

-- Tech/work
('Kate Morrison', 'MacBook Pro 2021', 'Primary work laptop for dissertation writing', '2021-01-01', 'purchased', 'Kate''s apartment desk', 'low', 'With external monitor, wireless keyboard and mouse.'),

-- Wall art
('Kate Morrison', 'Great Lakes watershed map', 'Framed. Only wall art in apartment besides running medals.', NULL, 'purchased', 'Kate''s apartment living room wall', 'medium', 'Evidence of professional identity in personal space.');

-- =====================================================
-- SCHEDULES TABLE - ADDITIONS
-- =====================================================

INSERT INTO schedules (
    character_name, schedule_type, day_of_week, start_time, end_time,
    activity, location, is_mandatory, notes
) VALUES
-- Lab meeting schedule
('Kate Morrison', 'weekly_commitment', 'Thursday', '14:00', '16:00', 'Lab meeting', 'Tech Building, Northwestern', 1, 'Data presentations, troubleshooting, paper planning. Kate prepares carefully. Presents every 3-4 weeks.'),
('Kate Morrison', 'weekly_commitment', 'Thursday', '15:00', '16:00', 'Thursday coffee social', 'Chen Lab kitchen', 0, 'Informal gathering. Patricia brings good coffee. Kate attends ~80% of time, stays 30-40 minutes, doesn''t stay for extended socializing.');

-- =====================================================
-- CHARACTER_NEGATIVES TABLE - ADDITIONS
-- =====================================================

INSERT INTO character_negatives (
    character_name, behavior, category, severity, exceptions, notes
) VALUES
-- Conference/networking negatives
('Kate Morrison', 'Does NOT attend student mixers at conferences', 'social', 'strong', NULL, 'Skipped AGU student mixer (too anxiety-inducing). Too overwhelming, too many people.'),
('Kate Morrison', 'Does NOT stay at networking events longer than 45-60 minutes', 'social', 'strong', 'If required for specific professional reason', 'Maximum capacity. Leaves after reasonable effort. Did minimum necessary, not maximum beneficial.'),
('Kate Morrison', 'Does NOT attend conference banquets or social dinners', 'social', 'strong', 'Required for committee or advisor', 'Skipped SETAC conference banquet, EPA mixer. Patricia frustrated but Kate "just can''t."'),
('Kate Morrison', 'Does NOT pursue connections made at conferences', 'social', 'preference', 'If professional collaboration clearly valuable', 'Exchanged cards with 8-10 people, followed up with 4-5 briefly. Pattern of letting connections drift.');

-- =====================================================
-- EDUCATION TABLE - ADDITIONS
-- =====================================================

INSERT INTO education (
    character_name, institution, degree, field, start_year, end_year,
    advisor, gpa, honors, notes
) VALUES
('Alice Thomas', 'University of Illinois Urbana-Champaign', 'B.A.', 'Advertising', 2012, 2016, NULL, 3.7, 'Dean''s List, cum laude', 'Minor in Psychology. Ad Club vice president senior year. Study abroad London Spring 2015. Met Steve senior year.'),
('Alice Thomas', NULL, NULL, 'Google Ads certification', NULL, NULL, NULL, NULL, NULL, 'Industry certification for advertising work.'),
('Alice Thomas', NULL, NULL, 'Facebook Blueprint certification', NULL, NULL, NULL, NULL, NULL, 'Industry certification for advertising work.'),
('Steve Thomas', 'Northwestern University', 'MBA', 'Business', 2015, 2017, NULL, NULL, 'Kellogg School', 'Met Alice while visiting UIUC friends January 2016.'),
('Erin O''Brien', 'Unknown', 'BSN', 'Nursing', NULL, NULL, NULL, NULL, NULL, 'Works at Northwestern Memorial Hospital.'),
('Michael O''Brien', 'Loyola University Chicago', 'M.D.', 'Medicine', 2023, 2027, NULL, NULL, 'Stritch School of Medicine', 'Year 3 medical student. Sean and Maria''s son.'),
('Katie Brennan', 'University of Chicago', 'MSW', 'Social Work', 2022, 2024, NULL, NULL, NULL, 'Marie and James''s daughter. Starting social work career.'),
('Tom Brennan', 'Unknown', 'B.S.', 'Computer Science', NULL, NULL, NULL, NULL, NULL, 'Software engineer. Marie and James''s son. Engaged to Jennifer.');

-- =====================================================
-- END OF ADDITIONS
-- =====================================================
