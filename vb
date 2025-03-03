    @Mock
    private RSDao rsDao;

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

    @Test
    void testSetRestrictionType() throws Exception {
        String expectedRestrictionType = "TYPE_1";
        String expectedOverrideRestrictionTypeName = "OVERRIDE_TYPE_1";
        
        rsReportGenerator.setRestrictionType(expectedRestrictionType, expectedOverrideRestrictionTypeName);

        // Using reflection to access the private field
        Field restrictionTypeField = RSReportGenerator.class.getDeclaredField("restrictionType");
        restrictionTypeField.setAccessible(true);
        String actualRestrictionType = (String) restrictionTypeField.get(rsReportGenerator);

        assertEquals(expectedRestrictionType, actualRestrictionType);
    }

    @Test
    void testDoInXMLStream() throws Exception {
        StaXBuilder staXBuilder = mock(StaXBuilder.class);
        Set<String> incorrectRICs = new HashSet<>();
        List<RestrictedSecurity> restrictions = new ArrayList<>();

        when(rsDao.fetchRestrictions(any(Connection.class), anySet())).thenReturn(restrictions);

        rsReportGenerator.doInXMLStream(xmlStreamWriter);

        verify(rsDao, times(1)).fetchRestrictions(any(Connection.class), anySet());
    }
