@Mock
    private RSDao rsDao;

    @Mock
    private RSDao fetchRestrictionTypeRestriction;  // Fix: Mock this dependency

    @Mock
    private Connection connection;

    @Mock
    private XMLStreamWriter xmlStreamWriter;

    @InjectMocks
    private RSReportGenerator rsReportGenerator;

    @BeforeEach
    void setUp() {
        rsReportGenerator = new RSReportGenerator();
    }

    private void setPrivateField(Object target, String fieldName, Object value) throws Exception {
        Field field = RSReportGenerator.class.getDeclaredField(fieldName);
        field.setAccessible(true);
        field.set(target, value);
    }

    @Test
    void testSetRestrictionType() throws Exception {
        // Setting restrictionType = "21" and overrideRestrictionTypeName = null
        rsReportGenerator.setRestrictionType("21", null);

        // Validate restrictionType
        Field restrictionTypeField = RSReportGenerator.class.getDeclaredField("restrictionType");
        restrictionTypeField.setAccessible(true);
        String actualRestrictionType = (String) restrictionTypeField.get(rsReportGenerator);
        assertEquals("21", actualRestrictionType);

        // Validate overrideRestrictionTypeName
        Field overrideField = RSReportGenerator.class.getDeclaredField("overrideRestrictionTypeName");
        overrideField.setAccessible(true);
        String actualOverride = (String) overrideField.get(rsReportGenerator);
        assertNull(actualOverride);  // Ensure it's null
    }

    @Test
    void testDoInXMLStream() throws Exception {
        // Fix: Ensure restrictionType is set before calling doInXMLStream
        setPrivateField(rsReportGenerator, "restrictionType", "21");
        setPrivateField(rsReportGenerator, "overrideRestrictionTypeName", null);
        setPrivateField(rsReportGenerator, "fetchRestrictionTypeRestriction", fetchRestrictionTypeRestriction);

        StaXBuilder staXBuilder = mock(StaXBuilder.class);
        Set<String> incorrectRICs = new HashSet<>();
        List<RestrictedSecurity> restrictions = new ArrayList<>();

        // Fix: Ensure the mock returns a non-null value
        when(fetchRestrictionTypeRestriction.fetchRestrictions(any(Connection.class), anySet()))
            .thenReturn(restrictions);

        rsReportGenerator.doInXMLStream(xmlStreamWriter);

        verify(fetchRestrictionTypeRestriction, times(1)).fetchRestrictions(any(Connection.class), anySet());
    }
