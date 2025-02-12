    public List<RestrictedSecurity> fetchRestrictions(Connection connection) throws SQLException {
        List<RestrictedSecurity> restrictions = new ArrayList<>();

        String query = SQL_RESTRICTIONS;
        System.out.println("SQL_RESTRICTIONS: " + query);

        try (PreparedStatement preparedStatement = connection.prepareStatement(query)) {
            preparedStatement.setString(1, "Debt");
            preparedStatement.setString(2, "I");
            preparedStatement.setString(3, "14");

            try (ResultSet resultSet = preparedStatement.executeQuery()) {
                System.out.println("Executing Query...");

                while (resultSet.next()) {
                    // Creating RestrictedSecurity object from result set
                    RestrictedSecurity restriction = new RestrictedSecurity(resultSet);
                    restrictions.add(restriction);
                }
            }
        }

        System.out.println("Fetched " + restrictions.size() + " restrictions.");
        return restrictions;
    }
