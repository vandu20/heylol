 @Mock
    private RSDao rsDao;

    @Mock
    private RSDao fetchRestrictionTypeRestriction;

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
        rsReportGenerator.setRestrictionType("21", null);

        Field restrictionTypeField = RSReportGenerator.class.getDeclaredField("restrictionType");
        restrictionTypeField.setAccessible(true);
        String actualRestrictionType = (String) restrictionTypeField.get(rsReportGenerator);
        assertEquals("21", actualRestrictionType);

        Field overrideField = RSReportGenerator.class.getDeclaredField("overrideRestrictionTypeName");
        overrideField.setAccessible(true);
        String actualOverride = (String) overrideField.get(rsReportGenerator);
        assertNull(actualOverride);
    }

    @Test
    void testDoInXMLStream() throws Exception {
        setPrivateField(rsReportGenerator, "restrictionType", "21");
        setPrivateField(rsReportGenerator, "overrideRestrictionTypeName", null);
        setPrivateField(rsReportGenerator, "fetchRestrictionTypeRestriction", fetchRestrictionTypeRestriction);

        StaXBuilder staXBuilder = mock(StaXBuilder.class);
        Set<String> incorrectRICs = new HashSet<>();
        List<RestrictedSecurity> restrictions = new ArrayList<>();

        // Fix: Use more general argument matchers
        when(fetchRestrictionTypeRestriction.fetchRestrictions(any(), any()))
            .thenReturn(restrictions);

        rsReportGenerator.doInXMLStream(xmlStreamWriter);

        verify(fetchRestrictionTypeRestriction, times(1)).fetchRestrictions(any(), any());
    }
