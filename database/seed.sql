-- Test Data for Supabase PostgreSQL Database
-- Run this after init_supabase.sql to populate tables with sample data
-- Note: This assumes auth.users table has corresponding entries

-- ========================
-- Note: Profiles table data is managed separately through Supabase auth
-- Replace the UUIDs below with your actual profile UUIDs from Supabase
-- Expected profiles:
-- ('MEMBER_UUID_HERE', 'zhan2621@flinders.edu.au', 'member')
-- ('ENTERPRISE_UUID_HERE', 'zhangsu1305@gmail.com', 'enterprise')
-- ('EXPERT_UUID_HERE', 'enpyth@outlook.com', 'expert')
-- ('ADMIN_UUID_HERE', 'fucssa2013@gmail.com', 'admin')

-- ========================
-- Insert members
-- ========================
INSERT INTO public.members (id) 
SELECT id FROM public.profiles WHERE role = 'member' LIMIT 1;

-- ========================
-- Insert enterprises
-- ========================
INSERT INTO public.enterprises (id, company_name) 
SELECT id, 'TechCorp Solutions' FROM public.profiles WHERE role = 'enterprise' LIMIT 1;

-- ========================
-- Insert experts
-- ========================
INSERT INTO public.experts (id, expertise_area) 
SELECT id, 'Artificial Intelligence' FROM public.profiles WHERE role = 'expert' LIMIT 1;

-- ========================
-- Insert admins
-- ========================
INSERT INTO public.admins (id) 
SELECT id FROM public.profiles WHERE role = 'admin' LIMIT 1;

-- ========================
-- Insert collaboration requests
-- ========================
INSERT INTO public.collaboration_requests (sender_id, receiver_id, details, status) 
SELECT 
    (SELECT id FROM public.profiles WHERE role = 'enterprise' LIMIT 1),
    (SELECT id FROM public.profiles WHERE role = 'expert' LIMIT 1),
    'We are looking for AI expertise to develop a machine learning solution for our customer service automation. The project involves natural language processing and chatbot development.',
    'Pending'::collaboration_status

UNION ALL

SELECT 
    (SELECT id FROM public.profiles WHERE role = 'expert' LIMIT 1),
    (SELECT id FROM public.profiles WHERE role = 'enterprise' LIMIT 1),
    'I have developed a novel AI algorithm for predictive analytics that could benefit your customer insights platform. Would you be interested in exploring a collaboration?',
    'Approved'::collaboration_status;

-- ========================
-- Insert academic products
-- ========================
INSERT INTO public.academic_products (expert_id, achievements, title, description) 
SELECT 
    (SELECT id FROM public.profiles WHERE role = 'expert' LIMIT 1),
    '{"publications": 15, "patents": 3, "awards": ["Best AI Research 2023", "Innovation Excellence Award"], "h_index": 28, "citations": 1250}'::jsonb,
    'Advanced Machine Learning Algorithms for Predictive Analytics',
    'A comprehensive study on novel machine learning approaches for predictive analytics in business applications.'

UNION ALL

SELECT 
    (SELECT id FROM public.profiles WHERE role = 'expert' LIMIT 1),
    '{"publications": 8, "patents": 1, "awards": ["AI Innovation Prize"], "h_index": 18, "citations": 650}'::jsonb,
    'Deep Learning Applications in Natural Language Processing',
    'Research on transformer models and their applications in understanding human language patterns.';

-- ========================
-- Verify data insertion
-- ========================
-- Check profile counts by role
SELECT 
    role,
    COUNT(*) as count
FROM public.profiles 
GROUP BY role
ORDER BY role;

-- Check collaboration request status distribution
SELECT 
    status,
    COUNT(*) as count
FROM public.collaboration_requests 
GROUP BY status
ORDER BY status;

-- Check academic products per expert
SELECT 
    p.email,
    p.role,
    COUNT(ap.id) as product_count
FROM public.profiles p
LEFT JOIN public.academic_products ap ON p.id = ap.expert_id
WHERE p.role = 'expert'
GROUP BY p.id, p.email, p.role
ORDER BY product_count DESC;

-- Check collaboration request relationships
SELECT 
    cr.id,
    sender.email as sender_email,
    sender.role as sender_role,
    receiver.email as receiver_email,
    receiver.role as receiver_role,
    cr.status,
    cr.created_at
FROM public.collaboration_requests cr
JOIN public.profiles sender ON cr.sender_id = sender.id
JOIN public.profiles receiver ON cr.receiver_id = receiver.id
ORDER BY cr.created_at DESC;
