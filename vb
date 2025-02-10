    private String fetchRestrictionType(Connection connection) throws SQLException {
        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTION_TYPE)) {
            preparedStatement.setString(1, "20"); // Restriction type code
            preparedStatement.setString(2, "I"); // Status code

            ResultSet resultSet = preparedStatement.executeQuery();

            if (resultSet.next()) {
                return resultSet.getString(1);
            }
        }
        return "UNKNOWN";
    }
