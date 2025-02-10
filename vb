    private static final Logger LOGGER = LoggerFactory.getLogger(RSReportGenerator.class);

    private String restrictionType;
    private String overrideRestrictionTypeName;

    public RSReportGenerator() {
        // Default Constructor
    }

    @Override
    public void setRestrictionType(String restrictionType, String overrideRestrictionTypeName) {
        this.restrictionType = restrictionType;
        this.overrideRestrictionTypeName = overrideRestrictionTypeName;
    }

    @Override
    public void doInXMLStream(XMLStreamWriter xtw) throws Exception {
        LOGGER.info("Generating XML Report...");

        // Validate restriction type
        if (restrictionType == null || restrictionType.isEmpty()) {
            throw new IllegalArgumentException("Restriction type is not specified.");
        }

        // Initialize StaXBuilder
        StaXBuilder staXBuilder = new StaXBuilder(xtw);
        Set<String> incorrectRICs = new HashSet<>();
        List<RestrictedSecurity> restrictions = List.of(
            new RestrictedSecurity("Sec1"),
            new RestrictedSecurity("Sec2"),
            new RestrictedSecurity("Sec3")
        );

        String finalRestrictionType = (overrideRestrictionTypeName != null) 
            ? overrideRestrictionTypeName 
            : restrictionType; // Use fetched restriction type

        LOGGER.info("Using Restriction Type: {}", finalRestrictionType);

        boolean hasDaoException = false;

        try {
            LOGGER.info("Fetching restriction type...");
            // In real implementation, fetch from rsDao.getRestrictionType(restrictionType)
        } catch (Exception e) {
            hasDaoException = true;
            LOGGER.error("Error fetching restriction type", e);
        }

        LOGGER.info("Creating report...");
        staXBuilder.startElement(ExtractorConstants.REPORT_DETAILS)
            .addElement(ExtractorConstants.TYPE_OF_RESTRICTION, finalRestrictionType)
            .addElement(ExtractorConstants.EXTRACT_TIMESTAMP, DateTimeHelper.formatCalendarGmt(DateTimeHelper.getGMTCalendar()))
            .addElement(ExtractorConstants.EXTRACT_STATUS, "SUCCESS"); // Always SUCCESS

        if (hasDaoException) {
            staXBuilder.addElement(ExtractorConstants.FAILURE_REASON, ExtractorConstants.DATASOURCE_ERROR);
        }

        staXBuilder.endElement(); // Close Report Details
        LOGGER.info("Report Details Created");

        // Generate the securities list
        generateRestrictedSecuritiesList(staXBuilder, restrictions);

        staXBuilder.endElement(); // Close XML root element
        LOGGER.info("Report generation completed.");
    }
