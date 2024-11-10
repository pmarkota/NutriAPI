using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "graphql");

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

            migrationBuilder.CreateSequence<int>(
                name: "seq_schema_version",
                schema: "graphql",
                cyclic: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    username = table.Column<string>(type: "character varying", nullable: true),
                    email = table.Column<string>(type: "character varying", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    goal = table.Column<string>(type: "character varying", nullable: true),
                    dietary_preference = table.Column<string>(type: "text", nullable: true),
                    caloric_goal = table.Column<long>(type: "bigint", nullable: true),
                    subscription_type = table.Column<string>(type: "character varying", nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("Users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MealPlans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    plan_name = table.Column<string>(type: "character varying", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    caloric_total = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("MealPlans_pkey", x => x.id);
                    table.ForeignKey(
                        name: "MealPlans_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    message = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying", nullable: true),
                    scheduled_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_sent = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Notifications_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Notifications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    amount = table.Column<float>(type: "real", nullable: true),
                    currency = table.Column<string>(type: "character varying", nullable: true),
                    payment_method = table.Column<string>(type: "character varying", nullable: true),
                    subscription_type = table.Column<string>(type: "character varying", nullable: true),
                    status = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Payments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Payments_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    ingredients = table.Column<string>(type: "jsonb", nullable: true),
                    instructions = table.Column<string>(type: "text", nullable: true),
                    calories = table.Column<long>(type: "bigint", nullable: true),
                    protein = table.Column<long>(type: "bigint", nullable: true),
                    carbohydrates = table.Column<long>(type: "bigint", nullable: true),
                    fats = table.Column<long>(type: "bigint", nullable: true),
                    dietary_labels = table.Column<string>(type: "character varying", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("Recipes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "Recipes_created_by_fkey",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    favorite_recipes = table.Column<string>(type: "text", nullable: false),
                    excluded_ingredients = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("UserPreferences_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "UserPreferences_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingLists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    meal_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    generated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ShoppingLists_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ShoppingLists_meal_plan_id_fkey",
                        column: x => x.meal_plan_id,
                        principalTable: "MealPlans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ShoppingLists_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanRecipes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    meal_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                    day = table.Column<string>(type: "character varying", nullable: true),
                    meal_type = table.Column<string>(type: "character varying", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("MealPlanRecipes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "MealPlanRecipes_meal_plan_id_fkey",
                        column: x => x.meal_plan_id,
                        principalTable: "MealPlans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "MealPlanRecipes_recipe_id_fkey",
                        column: x => x.recipe_id,
                        principalTable: "Recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingListItems",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    shopping_list_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ingredient_name = table.Column<string>(type: "character varying", nullable: true),
                    quantity = table.Column<float>(type: "real", nullable: true),
                    unit = table.Column<string>(type: "character varying", nullable: true),
                    is_checked = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ShoppingListItems_pkey", x => x.id);
                    table.ForeignKey(
                        name: "ShoppingListItems_shopping_list_id_fkey",
                        column: x => x.shopping_list_id,
                        principalTable: "ShoppingLists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanRecipes_meal_plan_id",
                table: "MealPlanRecipes",
                column: "meal_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanRecipes_recipe_id",
                table: "MealPlanRecipes",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlans_user_id",
                table: "MealPlans",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_user_id",
                table: "Payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_created_by",
                table: "Recipes",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_shopping_list_id",
                table: "ShoppingListItems",
                column: "shopping_list_id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_meal_plan_id",
                table: "ShoppingLists",
                column: "meal_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_user_id",
                table: "ShoppingLists",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealPlanRecipes");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ShoppingListItems");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "ShoppingLists");

            migrationBuilder.DropTable(
                name: "MealPlans");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropSequence(
                name: "seq_schema_version",
                schema: "graphql");
        }
    }
}
