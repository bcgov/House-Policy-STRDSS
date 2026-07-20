/* STR DSS Sprint 22 - Backfill listing actions from historical email messages
 *
 * Scope: email-sourced action types only (NonComplianceNotice, TakedownRequest,
 * PlatformTakedown, ComplianceOrder).
 * Repeated actions that were not recorded as emails are not backfilled.
 * Action types without email associations are populated by application hooks in future work.
 *
 * Prerequisite: ComplianceOrder row must exist in dss_listing_action_type (Sprint 22 seed).
 * Safe to re-run: skips rows already linked via source_email_message_id.
 */

INSERT INTO dss_rental_listing_action (
	rental_listing_id,
	listing_action_type,
	action_dtm,
	action_short_nm,
	action_long_nm,
	takedown_reason,
	initiating_user_identity_id,
	source_email_message_id
)
SELECT
	em.concerned_with_rental_listing_id,
	CASE em.email_message_type
		WHEN 'Notice of Takedown' THEN 'NonComplianceNotice'
		WHEN 'Takedown Request'    THEN 'TakedownRequest'
		WHEN 'Completed Takedown'  THEN 'PlatformTakedown'
		WHEN 'Compliance Order'    THEN 'ComplianceOrder'
	END,
	em.message_delivery_dtm,
	CASE
		WHEN em.email_message_type = 'Notice of Takedown' THEN 'notice of non-compliance'
		WHEN em.email_message_type = 'Takedown Request'    THEN 'takedown request'
		WHEN em.email_message_type = 'Compliance Order'    THEN 'compliance order'
		WHEN em.email_message_type = 'Completed Takedown'
			AND drl.takedown_reason = 'Invalid Registration' THEN 'Reg Check Failed'
		WHEN em.email_message_type = 'Completed Takedown'  THEN 'takedown reported'
	END,
	CASE
		WHEN em.email_message_type = 'Notice of Takedown' THEN 'Notice of Non-compliance'
		WHEN em.email_message_type = 'Takedown Request'    THEN 'Takedown Request'
		WHEN em.email_message_type = 'Compliance Order'    THEN 'Compliance Order'
		WHEN em.email_message_type = 'Completed Takedown'
			AND drl.takedown_reason = 'LG Request' THEN 'Takedown Reported: LG Request'
		WHEN em.email_message_type = 'Completed Takedown'
			AND drl.takedown_reason = 'Invalid Registration' THEN 'Takedown Reported: Registration Check Failed'
		WHEN em.email_message_type = 'Completed Takedown'  THEN 'Takedown Reported'
	END,
	CASE
		WHEN em.email_message_type = 'Completed Takedown' THEN drl.takedown_reason
		ELSE NULL
	END,
	em.initiating_user_identity_id,
	em.email_message_id
FROM dss_email_message em
INNER JOIN dss_rental_listing drl
	ON drl.rental_listing_id = em.concerned_with_rental_listing_id
WHERE em.concerned_with_rental_listing_id IS NOT NULL
  AND em.email_message_type IN ('Notice of Takedown', 'Takedown Request', 'Completed Takedown', 'Compliance Order')
  AND NOT EXISTS (
	SELECT 1
	FROM dss_rental_listing_action existing
	WHERE existing.source_email_message_id = em.email_message_id
  );
