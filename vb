@Test
void testFetchRestrictions() throws SQLException {
    // Arrange
    Set<String> incorrectRICs = new HashSet<>();

    // Mock ResultSet behavior to return one entry
    when(mockResultSet.next()).thenReturn(true, false); // First call: true, Second call: false (end of result set)
    when(mockResultSet.getString(1)).thenReturn("ID_123");

    // Creating a mock RestrictedSecurity object and placing it in the map
    RestrictedSecurity mockSecurity = mock(RestrictedSecurity.class);
    Map<String, RestrictedSecurity> restrictions = new HashMap<>();
    restrictions.put("ID_123", mockSecurity);

    // Ensure that fetchRestrictions() correctly retrieves the security
    when(restrictedSecurityService.process(any(ResultSet.class), eq(mockSecurity)))
        .thenAnswer(invocation -> {
            LOGGER.info("Processing restricted security...");
            return null;
        });

    // Act
    List<RestrictedSecurity> restrictionsList = rsDaoImpl.fetchRestrictions(mockConnection, incorrectRICs);

    // Assert
    assertNotNull(restrictionsList, "Restrictions list should not be null");
    assertFalse(restrictionsList.isEmpty(), "Restrictions list should not be empty");
    assertEquals(1, restrictionsList.size(), "Expected one restriction entry");

    // Verify method interactions
    verify(mockPreparedStatement, times(1)).setString(1, "RIC");
    verify(mockPreparedStatement, times(1)).setString(2, "A");
    verify(mockPreparedStatement, times(1)).setString(3, "21");
    verify(mockResultSet, times(1)).next();
    verify(restrictedSecurityService, times(1)).process(any(ResultSet.class), eq(mockSecurity));
}
