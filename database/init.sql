-- 初始化 Supabase PostgreSQL Database Schema
-- 在 Supabase SQL Editor 里运行

-- ========================
-- 扩展
-- ========================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ========================
-- 清理旧对象（防止重复执行报错）
-- ========================
DROP TABLE IF EXISTS public.admins CASCADE;
DROP TABLE IF EXISTS public.experts CASCADE;
DROP TABLE IF EXISTS public.enterprises CASCADE;
DROP TABLE IF EXISTS public.members CASCADE;
DROP TABLE IF EXISTS public.profiles CASCADE;
DROP TABLE IF EXISTS public.collaboration_requests CASCADE;
DROP TABLE IF EXISTS public.academic_products CASCADE;

DROP TYPE IF EXISTS user_role CASCADE;
DROP TYPE IF EXISTS collaboration_status CASCADE;

DROP FUNCTION IF EXISTS public.handle_new_user() CASCADE;
DROP FUNCTION IF EXISTS public.handle_update_user() CASCADE;

DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;
DROP TRIGGER IF EXISTS on_auth_user_updated ON auth.users;

-- ========================
-- 枚举类型
-- ========================
CREATE TYPE user_role AS ENUM ('member', 'enterprise', 'expert', 'admin');
CREATE TYPE collaboration_status AS ENUM ('Pending', 'Approved', 'Rejected');

-- ========================
-- profiles 总表
-- ========================
CREATE TABLE public.profiles (
    id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
    email VARCHAR(255) NOT NULL UNIQUE,
    role user_role NOT NULL DEFAULT 'member',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================
-- 子表（继承 profiles.id 外键）
-- ========================
CREATE TABLE public.members (
    id UUID PRIMARY KEY REFERENCES public.profiles(id) ON DELETE CASCADE
);

CREATE TABLE public.enterprises (
    id UUID PRIMARY KEY REFERENCES public.profiles(id) ON DELETE CASCADE,
    company_name VARCHAR(200) NOT NULL
);

CREATE TABLE public.experts (
    id UUID PRIMARY KEY REFERENCES public.profiles(id) ON DELETE CASCADE,
    expertise_area VARCHAR(200) NOT NULL
);

CREATE TABLE public.admins (
    id UUID PRIMARY KEY REFERENCES public.profiles(id) ON DELETE CASCADE
);

-- ========================
-- 业务表
-- ========================
CREATE TABLE public.collaboration_requests (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    sender_id UUID NOT NULL REFERENCES public.profiles(id) ON DELETE RESTRICT,
    receiver_id UUID NOT NULL REFERENCES public.profiles(id) ON DELETE RESTRICT,
    details TEXT NOT NULL CHECK (length(details) >= 10 AND length(details) <= 1000),
    status collaboration_status NOT NULL DEFAULT 'Pending',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE TABLE public.academic_products (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    expert_id UUID NOT NULL REFERENCES public.experts(id) ON DELETE CASCADE,
    achievements JSONB DEFAULT '{}',
    title VARCHAR(200) NOT NULL,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================
-- 索引
-- ========================
CREATE INDEX idx_profiles_email ON public.profiles(email);
CREATE INDEX idx_profiles_role ON public.profiles(role);
CREATE INDEX idx_collab_sender ON public.collaboration_requests(sender_id);
CREATE INDEX idx_collab_receiver ON public.collaboration_requests(receiver_id);
CREATE INDEX idx_collab_status ON public.collaboration_requests(status);
CREATE INDEX idx_products_expert ON public.academic_products(expert_id);
CREATE INDEX idx_products_achievements ON public.academic_products USING GIN (achievements);

-- ========================
-- updated_at 自动更新触发器
-- ========================
CREATE OR REPLACE FUNCTION public.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER trg_profiles_updated
BEFORE UPDATE ON public.profiles
FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER trg_collab_updated
BEFORE UPDATE ON public.collaboration_requests
FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER trg_products_updated
BEFORE UPDATE ON public.academic_products
FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

-- ========================
-- auth.users -> profiles 同步触发器
-- ========================
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
  INSERT INTO public.profiles (id, email, role)
  VALUES (NEW.id, NEW.email, 'member')
  ON CONFLICT (id) DO NOTHING;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

CREATE OR REPLACE FUNCTION public.handle_update_user()
RETURNS TRIGGER AS $$
BEGIN
  UPDATE public.profiles
  SET email = NEW.email,
      updated_at = NOW()
  WHERE id = NEW.id;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

CREATE TRIGGER on_auth_user_created
AFTER INSERT ON auth.users
FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();

CREATE TRIGGER on_auth_user_updated
AFTER UPDATE OF email ON auth.users
FOR EACH ROW
WHEN (OLD.email IS DISTINCT FROM NEW.email)
EXECUTE FUNCTION public.handle_update_user();

-- ========================
-- RLS (Row Level Security)
-- ========================
ALTER TABLE public.profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.members ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.enterprises ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.experts ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.admins ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.collaboration_requests ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.academic_products ENABLE ROW LEVEL SECURITY;

-- 用户只能查看/更新自己的 profile
CREATE POLICY "Users can view own profile" ON public.profiles
    FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Users can update own profile" ON public.profiles
    FOR UPDATE USING (auth.uid() = id);

-- 用户可以查看和管理与自己相关的 collaboration_requests
CREATE POLICY "Users can insert collab requests" ON public.collaboration_requests
    FOR INSERT WITH CHECK (auth.uid() = sender_id);

CREATE POLICY "Users can view related collab requests" ON public.collaboration_requests
    FOR SELECT USING (auth.uid() = sender_id OR auth.uid() = receiver_id);

CREATE POLICY "Users can update received collab requests" ON public.collaboration_requests
    FOR UPDATE USING (auth.uid() = receiver_id);

-- academic_products：公开可读，专家可写
CREATE POLICY "Anyone can view academic products" ON public.academic_products
    FOR SELECT USING (true);

CREATE POLICY "Experts can insert products" ON public.academic_products
    FOR INSERT WITH CHECK (auth.uid() = expert_id);

CREATE POLICY "Experts can update own products" ON public.academic_products
    FOR UPDATE USING (auth.uid() = expert_id);

-- ========================
-- 权限
-- ========================
GRANT USAGE ON SCHEMA public TO anon, authenticated;
GRANT ALL ON ALL TABLES IN SCHEMA public TO anon, authenticated;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO anon, authenticated;
