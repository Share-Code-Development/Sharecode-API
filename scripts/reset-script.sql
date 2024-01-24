-- Drop Views
DROP MATERIALIZED VIEW IF EXISTS snippet."MV_SnippetCommentReactions";
DROP MATERIALIZED VIEW IF EXISTS  snippet."MV_SnippetReactions";

-- Outbox & EF
DROP TABLE IF EXISTS outbox."OutboxMessages";
DROP TABLE IF EXISTS  public."__EFMigrationsHistory";

DROP TABLE IF EXISTS sharecode."AccountSetting";
DROP TABLE IF EXISTS sharecode."GatewayRequest";
DROP TABLE IF EXISTS sharecode."UserRefreshToken";

DROP TABLE IF EXISTS snippet."SnippetCommentReactions";
DROP TABLE IF EXISTS snippet."SnippetComments";
DROP TABLE IF EXISTS snippet."SnippetReactions";
DROP TABLE IF EXISTS snippet."SnippetAccessControls";
DROP TABLE IF EXISTS snippet."SnippetLineComments";
DROP TABLE IF EXISTS snippet."Snippets";

DROP TABLE IF EXISTS sharecode."User";