using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleAuthColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserPreferences",
                newName: "UserPreferences",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ShoppingLists",
                newName: "ShoppingLists",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ShoppingListItems",
                newName: "ShoppingListItems",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Recipes",
                newName: "Recipes",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payments",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notifications",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "MealPlans",
                newName: "MealPlans",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "MealPlanRecipes",
                newName: "MealPlanRecipes",
                newSchema: "public");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:auth.aal_level", "aal1,aal2,aal3")
                .OldAnnotation("Npgsql:Enum:auth.code_challenge_method", "s256,plain")
                .OldAnnotation("Npgsql:Enum:auth.factor_status", "unverified,verified")
                .OldAnnotation("Npgsql:Enum:auth.factor_type", "totp,webauthn,phone")
                .OldAnnotation("Npgsql:Enum:auth.one_time_token_type", "confirmation_token,reauthentication_token,recovery_token,email_change_token_new,email_change_token_current,phone_change_token")
                .OldAnnotation("Npgsql:Enum:pgsodium.key_status", "default,valid,invalid,expired")
                .OldAnnotation("Npgsql:Enum:pgsodium.key_type", "aead-ietf,aead-det,hmacsha512,hmacsha256,auth,shorthash,generichash,kdf,secretbox,secretstream,stream_xchacha20")
                .OldAnnotation("Npgsql:Enum:realtime.action", "INSERT,UPDATE,DELETE,TRUNCATE,ERROR")
                .OldAnnotation("Npgsql:Enum:realtime.equality_op", "eq,neq,lt,lte,gt,gte,in")
                .OldAnnotation("Npgsql:PostgresExtension:extensions.pg_stat_statements", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:extensions.pgcrypto", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:extensions.pgjwt", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:extensions.uuid-ossp", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:graphql.pg_graphql", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pgsodium.pgsodium", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:vault.supabase_vault", ",,");

            migrationBuilder.AddColumn<string>(
                name: "google_id",
                schema: "public",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "picture",
                schema: "public",
                table: "users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "google_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "picture",
                schema: "public",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "public",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "UserPreferences",
                schema: "public",
                newName: "UserPreferences");

            migrationBuilder.RenameTable(
                name: "ShoppingLists",
                schema: "public",
                newName: "ShoppingLists");

            migrationBuilder.RenameTable(
                name: "ShoppingListItems",
                schema: "public",
                newName: "ShoppingListItems");

            migrationBuilder.RenameTable(
                name: "Recipes",
                schema: "public",
                newName: "Recipes");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "public",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "public",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "MealPlans",
                schema: "public",
                newName: "MealPlans");

            migrationBuilder.RenameTable(
                name: "MealPlanRecipes",
                schema: "public",
                newName: "MealPlanRecipes");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:auth.aal_level", "aal1,aal2,aal3")
                .Annotation("Npgsql:Enum:auth.code_challenge_method", "s256,plain")
                .Annotation("Npgsql:Enum:auth.factor_status", "unverified,verified")
                .Annotation("Npgsql:Enum:auth.factor_type", "totp,webauthn,phone")
                .Annotation("Npgsql:Enum:auth.one_time_token_type", "confirmation_token,reauthentication_token,recovery_token,email_change_token_new,email_change_token_current,phone_change_token")
                .Annotation("Npgsql:Enum:pgsodium.key_status", "default,valid,invalid,expired")
                .Annotation("Npgsql:Enum:pgsodium.key_type", "aead-ietf,aead-det,hmacsha512,hmacsha256,auth,shorthash,generichash,kdf,secretbox,secretstream,stream_xchacha20")
                .Annotation("Npgsql:Enum:realtime.action", "INSERT,UPDATE,DELETE,TRUNCATE,ERROR")
                .Annotation("Npgsql:Enum:realtime.equality_op", "eq,neq,lt,lte,gt,gte,in")
                .Annotation("Npgsql:PostgresExtension:extensions.pg_stat_statements", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.pgjwt", ",,")
                .Annotation("Npgsql:PostgresExtension:extensions.uuid-ossp", ",,")
                .Annotation("Npgsql:PostgresExtension:graphql.pg_graphql", ",,")
                .Annotation("Npgsql:PostgresExtension:pgsodium.pgsodium", ",,")
                .Annotation("Npgsql:PostgresExtension:vault.supabase_vault", ",,");
        }
    }
}
