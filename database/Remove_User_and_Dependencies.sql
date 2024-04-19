delete from dss_email_message where :user_identity_id in (initiating_user_identity_id,affected_by_user_identity_id);
delete from dss_user_role_assignment where user_identity_id=:user_identity_id;
delete from dss_user_identity where user_identity_id=:user_identity_id;
