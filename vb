 public List<RestrictedSecurity> fetchRestrictions(Connection connection) throws SQLException {
        List<RestrictedSecurity> restrictions = new ArrayList<>();

        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTIONS)) {
            
            System.out.println("SQL_RESTRICTIONS: " + SQL_RESTRICTIONS);

            // Setting parameters
            preparedStatement.setString(1, "Debt");
            preparedStatement.setString(2, "I");
            preparedStatement.setString(3, "14");

            try (ResultSet resultSet = preparedStatement.executeQuery()) {
                
                while (resultSet.next()) {
                    RestrictedSecurity restriction = new RestrictedSecurity(
                        resultSet.getString(1), // Restriction ID
                        resultSet.getString(2), // Security Name
                        resultSet.getString(3), // ID Type
                        resultSet.getString(4)  // ID Value
                    );
                    restrictions.add(restriction);
                }
            }
        }
        return restrictions;
    }
