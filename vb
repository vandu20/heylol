import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.reactive.function.BodyInserters;
import org.springframework.web.reactive.function.client.WebClient;
import reactor.core.publisher.Flux;
import reactor.core.publisher.Mono;
import java.nio.charset.StandardCharsets;
import java.util.Base64;

@Component
public class TokenUtil {

    private static final Logger logger = LoggerFactory.getLogger(TokenUtil.class);

    private static final String GRANT_TYPE = "grant_type";
    private static final String SCOPE = "scope";
    public static final String CONTENT_TYPE = "Content-Type";
    public static final String X_FORM_URL_ENC_CONTENT_TYPE = "application/x-www-form-urlencoded";
    public static final String AUTHORIZATION_HEADER = "Authorization";
    public static final String AUTHORIZATION_HEADER_BASIC = "Basic ";

    @Value("${dbcruise.grant.type}")
    private String grantType;

    @Value("${dbcruise.token.url}")
    private String tokenUrl;

    @Value("${dbcruise.scope}")
    private String scope;

    @Value("${dbcruise.client.id}")
    private String clientId;

    @Value("${dbcruise.client.secret}")
    private String clientSecret;

    @Autowired
    private WebClient webClient;

    public Mono<DbCruiseToken> getJWTToken() {
        logger.info("Fetching dbCruise Token...");

        MultiValueMap<String, String> formData = new LinkedMultiValueMap<>();
        formData.add(GRANT_TYPE, grantType);
        formData.add(SCOPE, scope);

        String credentials = clientId + ":" + clientSecret;
        String encodedCredentials = Base64.getEncoder().encodeToString(credentials.getBytes(StandardCharsets.UTF_8));

        return webClient.post()
            .uri(tokenUrl)
            .header(CONTENT_TYPE, X_FORM_URL_ENC_CONTENT_TYPE)
            .header(AUTHORIZATION_HEADER, AUTHORIZATION_HEADER_BASIC + encodedCredentials)
            .body(BodyInserters.fromFormData(formData))
            .retrieve()
            .bodyToMono(DbCruiseToken.class)
            .doOnSuccess(token -> logger.info("Token fetched successfully: {}", token))
            .doOnError(error -> logger.error("Error fetching token", error));
    }

    public Flux<MyItem> fetchAndProcessData(String dataUrl) {
        return getJWTToken()
            .flatMapMany(token -> webClient.get()
                .uri(dataUrl)
                .header(AUTHORIZATION_HEADER, "Bearer " + token.getAccessToken())
                .retrieve()
                .bodyToFlux(MyItem.class)
            )
            .doOnNext(item -> processChunk(item))  // Processing each chunk
            .doOnError(error -> logger.error("Error processing JSON: {}", error.getMessage()))
            .doOnComplete(() -> logger.info("Completed processing JSON"));
    }

    private void processChunk(MyItem chunk) {
        logger.info("Processed: {}", chunk);
    }

    public void startProcessing(String dataUrl) {
        Flux<MyItem> responseFlux = fetchAndProcessData(dataUrl);

        responseFlux.subscribe(
            chunk -> processChunk(chunk), // Process each chunk
            error -> logger.error("Error: {}", error.getMessage()),
            () -> logger.info("Completed processing JSON")
        );
    }
}
