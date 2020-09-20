import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
  stages: [
    { duration: '2m', target: 500 }, // below normal load
    { duration: '3m', target: 500 },
    { duration: '2m', target: 1000 }, // normal load
    { duration: '3m', target: 1000 },
    { duration: '2m', target: 1500 }, // around the breaking point
    { duration: '3m', target: 1500 },
    { duration: '2m', target: 1900 }, // beyond the breaking point
    { duration: '3m', target: 1900 },
    { duration: '3m', target: 0 }, // scale down. Recovery stage.
  ],
};

const BASE_URL = 'http://HAProxyClusterLB.drinkpoint.me';

const PAYLOAD = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ullamcorper sed tellus non imperdiet. Vestibulum at nisi ultricies, dapibus quam quis, ullamcorper ex. Duis pulvinar ut felis at tincidunt. Nunc mattis nisl nec diam mattis sodales. Praesent maximus in nibh non varius. Cras elit leo, vulputate non enim non, tempus vestibulum purus. Phasellus at nibh sed risus bibendum sollicitudin. Nullam suscipit lectus vitae luctus molestie. Donec ac orci quis neque pretium feugiat vitae ac sapien. Vestibulum at justo lacus.Fusce vel euismod justo, ut blandit justo. Mauris tristique tortor felis, at ultricies nisi mattis at. Curabitur purus sapien, imperdiet a gravida in, tempus ac odio. Morbi quis lobortis felis, ac congue lectus. Donec a nunc in enim interdum aliquet. Aenean quis nunc quis risus cursus facilisis. Nulla tempor tempor magna, et sodales ante tempor sed. Quisque sed eros nec metus luctus congue et vitae lacus.Phasellus id nisl mattis, interdum ante eu, consectetur turpis. Nunc justo lectus, dictum quis ante eget, porttitor condimentum dolor. Cras placerat nibh quis feugiat ullamcorper. Nullam ante eros, imperdiet ut nisl non, mollis aliquet nisi. Integer sem eros, finibus non ligula sit amet, imperdiet tincidunt nunc. Aenean at nulla velit. Ut pulvinar massa varius, posuere tortor eu, malesuada mi. Donec et massa odio. Aliquam erat volutpat. Cras commodo ligula at purus pulvinar, vulputate lobortis lorem sagittis. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Sed hendrerit ligula malesuada, maximus nunc a, efficitur ipsum. Fusce gravida sem vehicula mollis blandit.Cras tristique id elit quis blandit. In hendrerit felis sed ipsum euismod, nec auctor leo semper. Curabitur pulvinar non est eu faucibus. Sed pellentesque odio eget lobortis faucibus. Donec vitae fringilla massa. Duis sagittis aliquet nisl, sit amet volutpat metus facilisis quis. Donec pellentesque, odio eu dignissim venenatis, ex odio gravida sem, sed mollis mauris velit sit amet dolor. Donec euismod consequat erat vitae efficitur. Vestibulum pulvinar massa tempus, gravida mauris ac, dictum turpis. Fusce lobortis auctor sapien, et elementum mi gravida vel. Nulla mattis enim id enim molestie accumsan.In semper diam sit amet neque dignissim commodo. Cras eget nisi dignissim lectus interdum fermentum. Integer ac ipsum nec tortor mollis volutpat ac id libero. Sed imperdiet est odio, sed viverra diam venenatis non. Donec laoreet, arcu sed porta convallis, nunc elit commodo leo, quis faucibus urna urna sit amet odio. Integer condimentum bibendum diam, vel ultrices eros auctor id. Fusce sodales tortor vel molestie gravida. Vestibulum feugiat eu ipsum et suscipit.Duis a metus justo. Vivamus leo enim, blandit quis odio vitae, faucibus scelerisque magna. Nam ut elementum mi, in interdum elit. Donec blandit ligula at mattis posuere. Maecenas rhoncus nulla vel justo blandit aliquam non sit amet risus. Etiam commodo risus sit amet orci aliquet gravida. Praesent a diam eu nibh scelerisque fermentum. Vivamus elementum mauris ullamcorper, aliquet mi et, cursus ligula. Pellentesque a orci ut dui sagittis viverra. Vivamus non varius tortor. Nulla vel pulvinar diam. Morbi scelerisque lacus eget diam fringilla, nec aliquam ipsum convallis. Donec pulvinar.';

export default function () {

  let headerCollection = { headers: {
    Host: `owo-converter.owo-converter.drinkpoint.me`
  }};

  let response = http.get(`${BASE_URL}/${PAYLOAD}`, headerCollection);
  check(response.status, { 'is status 200': (code) => code === 200});
}