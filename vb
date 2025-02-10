@Override
    public void run(String... args) throws Exception {
        try (Connection connection = DriverManager.getConnection(URL, USER, PASSWORD)) {
            String restrictionType = fetchRestrictionType(connection);
            
            // Call RSReportGenerator to generate XML report
            RSReportGenerator reportGenerator = new RSReportGenerator();
            reportGenerator.generateXMLReport(restrictionType);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    private String fetchRestrictionType(Connection connection) throws SQLException {
        String restrictionType = "DEFAULT"; // Default fallback value
        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTION_TYPE)) {
            preparedStatement.setString(1, "20"); // restriction type code
            preparedStatement.setString(2, "I");  // status code

            try (ResultSet resultSet = preparedStatement.executeQuery()) {
                if (resultSet.next()) {
                    restrictionType = resultSet.getString(1);
                    System.out.println("Fetched Restriction Type: " + restrictionType);
                }
            }
        }
        return restrictionType;
    }
