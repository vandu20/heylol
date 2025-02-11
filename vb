@Override
public void run(String... args) throws Exception {
    try (Connection connection = DriverManager.getConnection(URL, USER, PASSWORD)) {
        String restrictionType = fetchRestrictionType(connection);
        LOGGER.info("Fetched restriction type: " + restrictionType);

        // Generate and save report
        String filePath = generateRSReport(restrictionType, null);
        LOGGER.info("Restricted securities report generated at: " + filePath);

    } catch (SQLException e) {
        LOGGER.error("Database error: " + e.getMessage(), e);
    }
}
