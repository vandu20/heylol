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
    void testSetRestrictionType() {
        rsReportGenerator.setRestrictionType("TYPE_1", "OVERRIDE_TYPE_1");
        
        assertEquals("TYPE_1", rsReportGenerator.restrictionType);
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
