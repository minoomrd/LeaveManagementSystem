# Leave Management System – Full Specification (Including UI Calendar + Billing Panel)

---

# 1. Overview
The Leave Management System allows organizations to define employees, configure working hours, set leave policies, and manage leave requests. The system includes:

- Calendar-based leave selection UI  
- Admin leave approval  
- Sick leave support  
- Working-hour based leave rules  
- Global Super-Admin Billing Panel  
- Pricing tiers & payment management  

---

# 2. System Features

## 2.1 Authentication
- Admin login  
- (Optional) Employee login  
- Super-admin login (for platform owners)

---

## 2.2 Employee Management (Company Admin)
Admins of each company can:

- Create/edit employees  
- Assign working hours (daily/weekly/monthly/yearly)  
- Define leave entitlements per employee  
- View leave history & balances  

---

# 3. Leave Management

## 3.1 Leave Type Configuration

### Standard Leave
- Leave may be **daily** or **hourly**
- Admin defines:
  - Allowed amount
  - Renewal period (weekly, monthly, yearly)

### Sick Leave
- Dedicated leave type
- Hourly or daily
- Admin defines:
  - Monthly or yearly entitlement
  - Whether medical proof is required

---

## 3.2 Leave Request Management

Employees can submit:
- Daily leave  
- Hourly leave  
- Sick leave  

Admins can:
- Approve
- Reject
- Comment

The system updates employee leave balance automatically after approval.

---

## 3.3 Calendar-Based UI for Leave Selection

The employee leave request UI includes a **calendar widget** where users can visually select dates.

### Features:
- Select **start date** and **end date** using the calendar
- For hourly leave → choose date + start time + end time
- Highlight:
  - Holidays (optional)
  - Weekends (optional)
  - Already approved leaves
- Auto-calculation of duration:
  - Total days
  - Total hours for hourly leave
- Validations:
  - Cannot select past dates
  - Cannot overlap existing leave
  - Cannot exceed leave balance

---

# 4. Leave Balance Calculation

The system supports:
- Hourly-based leave entitlement
- Daily leave entitlement  
- Monthly, weekly, or yearly resets

Example policies:
- “2 days per month standard leave”
- “20 hours per month”
- “30 days per year + 5 sick days”

The system automatically maintains:
- Used leave
- Remaining leave
- Auto-reset based on renewal period

---

# 5. Data Model (Database Tables)

## 5.1 `users`
Stores both admins and employees.

| Column | Type | Description |
|--------|------|-------------|
| id | UUID/int | PK |
| full_name | string | Name |
| email | string | Unique |
| password_hash | string | Credential |
| role | enum(admin, employee, super_admin) | Role |
| status | enum(active, inactive) | Status |
| created_at | datetime | |
| updated_at | datetime | |

---

## 5.2 `leave_types`
Defines categories of leave.

| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| name | string |
| unit | enum(hour, day) |
| description | text |
| is_sick_leave | boolean |
| created_at | datetime |
| updated_at | datetime |

---

## 5.3 `leave_policies`
Defines entitlements for each leave type.

| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| leave_type_id | FK |
| entitlement_amount | decimal |
| entitlement_unit | enum(day, hour) |
| renewal_period | enum(weekly, monthly, yearly) |
| created_at | datetime |
| updated_at | datetime |

---

## 5.4 `employee_leave_settings`
Custom entitlements per employee.

| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| user_id | FK |
| leave_type_id | FK |
| custom_entitlement_amount | decimal |
| custom_entitlement_unit | enum(day, hour) |
| created_at | datetime |
| updated_at | datetime |

---

## 5.5 `leave_requests`
Employee leave requests.

| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| user_id | FK |
| leave_type_id | FK |
| start_datetime | datetime |
| end_datetime | datetime |
| duration_amount | decimal |
| duration_unit | enum(day, hour) |
| reason | text |
| status | enum(pending, approved, rejected) |
| admin_comment | text |
| created_at | datetime |
| updated_at | datetime |

---

## 5.6 `leave_balances`
Tracks remaining leave.

| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| user_id | FK |
| leave_type_id | FK |
| balance_amount | decimal |
| balance_unit | enum(day, hour) |
| updated_at | datetime |

---

## 5.7 `working_hours`
Defines employee work schedule.

| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| user_id | FK |
| type | enum(daily, weekly, monthly, yearly) |
| hours | decimal |
| created_at | datetime |
| updated_at | datetime |

---

# 6. Super Admin Billing Panel (Platform Owner)

A dedicated **super-admin dashboard** manages:

- Companies using the platform  
- User limits  
- Billing plans  
- Payments  
- Pricing tiers  

---

## 6.1 Company Subscription & User Limits

Super-admin can:
- Set **free user limit** (e.g., 5 users free)
- Price per user range (tiers)
- Billing period (monthly/yearly)
- View company usage
- View company payments

### Example Tier Structure

| User Range | Price |
|-----------|--------|
| 0–5 | Free |
| 6–8 | €5 |
| 9–15 | €20 |
| 16–25 | €40 |
| 25+ | Custom pricing |

These can be configured dynamically.

---

## 6.2 Company Overview Page

Super-admin can view:

### Company Info
- Company name  
- Contact email  
- Registration date  
- Status (active/suspended)

### Usage Info
- Total number of users  
- Free tier limit  
- Current pricing tier  
- Next bill amount  
- Payment history  

### Admin Actions
- Change free tier limit  
- Move company to a different pricing tier  
- Apply discount  
- Suspend the company  
- View logs  

---

# 7. Payment System

The platform supports payments (e.g., Stripe).

### Features:
- Recurring monthly or yearly billing  
- Automatic calculation of cost based on active users  
- Payment history tracking  
- Invoice generation  
- Automatic notifications (failed payment / exceeded limits)  
- Optional: auto-suspend company if overdue  

---

# 8. Billing Database Tables

## 8.1 `companies`
| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| name | string |
| owner_id | FK → users.id |
| status | enum(active, suspended) |
| created_at | datetime |

---

## 8.2 `company_user_limits`
| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| company_id | FK |
| free_user_limit | int |
| created_at | datetime |
| updated_at | datetime |

---

## 8.3 `pricing_tiers`
| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| min_users | int |
| max_users | int |
| price | decimal |
| currency | string |
| created_at | datetime |

---

## 8.4 `company_subscriptions`
| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| company_id | FK |
| billing_period | enum(monthly, yearly) |
| current_tier_id | FK |
| next_billing_date | datetime |
| amount | decimal |
| status | enum(active, unpaid, canceled) |
| created_at | datetime |

---

## 8.5 `payments`
| Column | Type | Description |
|--------|------|-------------|
| id | PK |
| company_id | FK |
| amount | decimal |
| currency | string |
| billing_period | string |
| paid_at | datetime |
| payment_method | string |
| status | enum(success, failed) |

---

# 9. UI Summary

## Employee UI
- Calendar-based leave picker  
- Hourly/daily leave mode  
- Balance preview  
- Leave history  

## Company Admin UI
- Employee management  
- Leave approvals  
- Leave configuration  
- Working hours settings  

## Super Admin UI
- Company list  
- Pricing tier editor  
- Payments & invoices  
- Company usage analytics  

---

# 10. Summary

This document includes:
- Complete leave management system spec  
- Calendar UI requirements  
- Sick leave support  
- Database schema  
- Full Billing Panel + payment system  
- Company-tier subscription rules  

If you want, I can generate:
- **ERD diagram**  
- **API endpoints**  
- **Full UI/UX wireframes**  
- **React components**  
- **Prisma schema**  
- **Laravel migrations**  
