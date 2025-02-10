public class RSReportGenerator {
    private static final Logger LOGGER = LoggerFactory.getLogger(RSReportGenerator.class);

    public void generateXMLReport(String restrictionType) {
        try {
            // Simulated XMLStreamWriter (Replace this with actual writer)
            XMLStreamWriter xtw = new StaXBuilder(); 

            LOGGER.info("Generating XML report...");

            StaXBuilder staXBuilder = new StaXBuilder(xtw);

            // Create XML report details
            generateReportDetails(staXBuilder, restrictionType);

            // Simulating restricted securities list
            List<String> restrictions = List.of("Sec1", "Sec2", "Sec3");  
            generaterestrictedSecuritiesList(staXBuilder, restrictions);

            staXBuilder.endElement(); // Close "restrictedSecurityList" element
            LOGGER.info("Report generation completed successfully");

        } catch (Exception e) {
            LOGGER.error("Error while generating XML report", e);
        }
    }

    protected void generateReportDetails(StaXBuilder staXBuilder, String restrictionType) throws XMLStreamException {
        LOGGER.debug("Creating report details...");

        String extractTimestamp = DateTimeHelper.formatCalendarGmt(DateTimeHelper.getGMTCalendar());
        String status = ExtractorConstants.SUCCESS; // Always "SUCCESS"

        staXBuilder.startElement(ExtractorConstants.REPORT_DETAILS)
            .addElement(ExtractorConstants.TYPE_OF_RESTRICTION, restrictionType)
            .addElement(ExtractorConstants.EXTRACT_TIMESTAMP, extractTimestamp)
            .addElement(ExtractorConstants.EXTRACT_STATUS, status);

        LOGGER.debug("Report details have been created successfully");
        staXBuilder.endElement();
    }

    protected void generaterestrictedSecuritiesList(StaXBuilder staXBuilder, List<String> restrictions) throws XMLStreamException {
        LOGGER.debug("Generating restricted securities list...");

        staXBuilder.startElement("RestrictedSecurities");

        for (String security : restrictions) {
            staXBuilder.addElement("Security", security);
        }

        staXBuilder.endElement();
        LOGGER.debug("Restricted securities list added successfully.");
    }
}
