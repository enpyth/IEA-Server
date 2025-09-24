-- Clean All Data from Supabase PostgreSQL Database
-- This script removes all data from all tables while preserving the schema structure
-- Run this to reset the database to an empty state

-- ========================
-- Clean up all data in tables
-- ========================

-- Clean business tables first (due to foreign key constraints)
DELETE FROM public.academic_products;
DELETE FROM public.collaboration_requests;

-- Clean role-specific tables
DELETE FROM public.admins;
DELETE FROM public.experts;
DELETE FROM public.enterprises;
DELETE FROM public.members;

-- Clean base profiles table last
DELETE FROM public.profiles;

-- ========================
-- Verify data cleanup
-- ========================

-- Check that all tables are empty
SELECT 'profiles' as table_name, COUNT(*) as record_count FROM public.profiles
UNION ALL
SELECT 'members', COUNT(*) FROM public.members
UNION ALL
SELECT 'enterprises', COUNT(*) FROM public.enterprises
UNION ALL
SELECT 'experts', COUNT(*) FROM public.experts
UNION ALL
SELECT 'admins', COUNT(*) FROM public.admins
UNION ALL
SELECT 'collaboration_requests', COUNT(*) FROM public.collaboration_requests
UNION ALL
SELECT 'academic_products', COUNT(*) FROM public.academic_products
ORDER BY table_name;

-- Display success message
SELECT 'All data has been successfully cleaned from all tables.' as status;
